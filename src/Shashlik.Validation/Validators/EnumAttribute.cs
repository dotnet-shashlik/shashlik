using System;
using System.ComponentModel.DataAnnotations;
using Guc.Utils.Common;
using Guc.Utils.Extensions;

namespace Guc.Validation
{
    /// <summary>
    /// 枚举值验证
    /// </summary>
    public class EnumAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var type = value.GetType();
            if (!type.IsEnum)
                return ValidationResult.Success;

            if (!Enum.IsDefined(type, value))
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName} invalid enum value.");

            return ValidationResult.Success;
        }
    }
}