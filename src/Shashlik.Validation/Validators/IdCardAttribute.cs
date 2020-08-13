using System.ComponentModel.DataAnnotations;
using Guc.Utils.Common;

namespace Guc.Validation
{
    /// <summary>
    /// 身份证号码验证
    /// </summary>
    public class IdCardAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (!IdCardHelper.IsIdCard(value?.ToString()))
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}格式错误");
            return ValidationResult.Success;
        }
    }
}