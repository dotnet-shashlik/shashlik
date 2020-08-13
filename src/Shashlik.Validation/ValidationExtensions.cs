using Guc.Utils.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Guc.Validation
{
    public static class ValidationExtensions
    {
        /// <summary>
        /// 验证上下文获取方法
        /// </summary>
        private static Func<IServiceProvider> validationContextServiceProvider { get; set; }

        /// <summary>
        /// 设置模型验证上下文
        /// </summary>
        /// <param name="app"></param>
        /// <param name="validationContextServiceProvider"></param>
        public static void SetGucValidation(Func<IServiceProvider> validationContextServiceProvider)
        {
            if (ValidationExtensions.validationContextServiceProvider != null)
                throw new Exception("Validation can't repeat initialization.");
            ValidationExtensions.validationContextServiceProvider = validationContextServiceProvider;
        }

        /// <summary>
        /// 设置模型验证上下文
        /// </summary>
        /// <param name="app"></param>
        /// <param name="validationServiceContext"></param>
        public static void SetGucValidation(IServiceProvider serviceProvider)
        {
            if (validationContextServiceProvider != null)
                throw new Exception("Validation can't repeat initialization.");
            validationContextServiceProvider = () => serviceProvider;
        }

        /// <summary>
        /// 模型验证
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="maxErrorDepth"></param>
        /// <returns></returns>
        public static ValidResult IsValid<TModel>(this TModel model, int maxValidationDepth = 1)
            where TModel : class
        {
            Dictionary<object, ValidResult> validated = new Dictionary<object, ValidResult>();
            IsValid(model, maxValidationDepth, validated);
            if (validated.Any(r => !r.Value.Success))
            {
                // 有一个不成功的就验证失败

                var errors = validated.Where(r => !r.Value.Success).SelectMany(r => r.Value.ValidationResults);
                return new ValidResult
                {
                    Success = false,
                    Error = errors.Select(r => r.ErrorMessage).Join(Environment.NewLine),
                    ValidationResults = validated.Where(r => !r.Value.Success).SelectMany(r => r.Value.ValidationResults)
                };
            }
            else
            {
                // 验证成功
                return new ValidResult { Success = true };
            }
        }

        /// <summary>
        /// 模型验证
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="maxErrorCount"></param>
        /// <param name="validated"></param>
        /// <returns></returns>
        private static void IsValid<TModel>(this TModel model, int maxErrorCount, Dictionary<object, ValidResult> validated)
            where TModel : class
        {
            if (validated.Count(r => !r.Value.Success) >= maxErrorCount || validated.ContainsKey(model))
                return;
            IServiceProvider serviceProvider = null;
            if (validationContextServiceProvider != null)
                serviceProvider = validationContextServiceProvider();

            var context = new ValidationContext(model, serviceProvider: serviceProvider, items: null);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, validationResults, true);
            var type = model.GetType().GetTypeInfo();

            var result =
                new ValidResult
                {
                    Success = isValid,
                    ValidationResults = validationResults
                };

            validated.Add(model, result);

            foreach (var pro in type.GetProperties_())
            {
                if (pro.ReflectedType.IsSimpleType() || pro.ReflectedType.IsChildTypeOf<JToken>())
                    continue;
                object value;
                try
                {
                    value = pro.GetValue(model);
                }
                catch { continue; }
                if (value == null)
                    continue;

                var list = value as IEnumerable;
                if (list != null)
                {
                    // 集合
                    foreach (var child in list)
                    {
                        if (child == null)
                            continue;
                        if (child.GetType().IsSimpleType())
                            continue;

                        IsValid(child, maxErrorCount, validated);
                    }
                }
                else
                {
                    // 类
                    IsValid(value, maxErrorCount, validated);
                }
            }
        }

        /// <summary>
        /// 获取public/instance/getfield属性,并排除默认成员DefaultMember
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        static List<PropertyInfo> GetProperties_(this Type type)
        {
            var ps =
            type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField)
                .Where(r => !r.GetIndexParameters().Any())
                .ToList();

            return ps;
        }
    }

    public class ValidResult
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 错误信息,多个错误 换行符分割
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// 验证结果
        /// </summary>
        public IEnumerable<ValidationResult> ValidationResults { get; set; }
    }
}
