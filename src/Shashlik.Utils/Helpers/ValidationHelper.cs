using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// 递归模型验证
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="validationServiceProvider">服务上下文</param>
        /// <param name="maxErrorCount">最大错误数量,null全部验证</param>
        /// <param name="maxValidationDepth">最大递归验证深度,null全部验证</param>
        /// <returns></returns>
        public static List<ValidationResult> Validate(
            object model,
            IServiceProvider validationServiceProvider = null,
            int? maxErrorCount = null,
            int? maxValidationDepth = null)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));
            var results = new List<ValidationResult>();
            Validate(model, validationServiceProvider, maxErrorCount, maxValidationDepth, 1, results);
            return results;
        }

        private static void Validate(
            object model,
            IServiceProvider validationServiceProvider,
            int? maxErrorCount,
            int? maxValidationDepth,
            int hasDepth,
            List<ValidationResult> results
        )
        {
            if (model is null)
                return;
            if (maxValidationDepth.HasValue && hasDepth > maxValidationDepth)
                return;
            if (maxErrorCount.HasValue && results.Count > maxErrorCount)
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
                        Validate(ele, validationServiceProvider, maxErrorCount, maxValidationDepth, hasDepth + 1,
                            results);
                    }

                    continue;
                }

                // 普通对象,递归继续验证
                Validate(value, validationServiceProvider, maxErrorCount, maxValidationDepth, hasDepth + 1, results);
            }
        }
    }
}