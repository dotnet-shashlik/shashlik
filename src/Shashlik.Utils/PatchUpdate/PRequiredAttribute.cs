using System.ComponentModel.DataAnnotations;
using Guc.Utils.Extensions;
using System.Linq;

namespace Guc.Utils.PatchUpdate
{
    /// <summary>
    /// PatchUpdate非空验证,有该数据时才验证
    /// </summary>
    public class PRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var obj = validationContext.ObjectInstance as PatchUpdateBase;
            if (obj == null)
                return ValidationResult.Success;

            foreach (var item in obj.Origin)
            {
                if (item.Key.EqualsIgnoreCase(validationContext.MemberName))
                {
                    // 存在该属性
                    if (!base.IsValid(value))
                        return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}不能为空");
                    return ValidationResult.Success;
                }
            }

            // 不存在该属性,不验证
            return ValidationResult.Success;
        }
    }
}