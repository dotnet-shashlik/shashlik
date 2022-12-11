using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Shashlik.Utils.Extensions
{
    /// <summary>
    /// 递归模型验证
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// 递归模型验证
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="validationServiceProvider">服务上下文</param>
        /// <param name="maxValidationDepth">最大递归验证深度,null全部验证</param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static bool TryValidateObjectRecursion(
            object? model,
            IServiceProvider? validationServiceProvider,
            int maxValidationDepth,
            out List<ValidationResult> results)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            results = new List<ValidationResult>();
            Validate(model, validationServiceProvider, maxValidationDepth, 1, results);
            return !results.Any();
        }

        /// <summary>
        /// 递归模型验证
        /// </summary>
        /// <param name="model"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public static bool TryValidateObjectRecursion(this object? model, out List<ValidationResult> results)
        {
            return TryValidateObjectRecursion(model, null, 32, out results);
        }

        private static void Validate(
            object? model,
            IServiceProvider? validationServiceProvider,
            int? maxValidationDepth,
            int hasDepth,
            List<ValidationResult> results
        )
        {
            if (model is null)
                return;
            if (maxValidationDepth.HasValue && hasDepth > maxValidationDepth)
                return;

            var context = new ValidationContext(model, serviceProvider: validationServiceProvider, items: null);
            Validator.TryValidateObject(model, context, results, true);

            var type = model.GetType().GetTypeInfo();
            foreach (var declaredProperty in type.DeclaredProperties)
            {
                if (declaredProperty.GetIndexParameters().Any())
                    continue;

                if (!declaredProperty.CanRead)
                    continue;

                if (declaredProperty.PropertyType.IsSimpleType())
                    continue;

                var value = declaredProperty.GetValue(model);
                if (value is null)
                    continue;

                // 集合,枚举所有元素进行验证
                if (value is IEnumerable enumerable)
                {
                    foreach (var ele in enumerable)
                    {
                        if (ele.GetType().IsSimpleType())
                            continue;
                        Validate(ele, validationServiceProvider, maxValidationDepth, hasDepth + 1,
                            results);
                    }

                    continue;
                }

                // 普通对象,递归继续验证
                Validate(value, validationServiceProvider, maxValidationDepth, hasDepth + 1, results);
            }
        }
    }
}