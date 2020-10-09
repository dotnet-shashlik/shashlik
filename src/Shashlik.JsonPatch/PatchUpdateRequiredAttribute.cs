using System.ComponentModel.DataAnnotations;
using Shashlik.Utils.Extensions;

// ReSharper disable InvertIf

namespace Shashlik.JsonPatch
{
    /// <summary>
    /// PatchUpdate非空验证,有该数据时才验证
    /// </summary>
    public class PatchUpdateRequiredAttribute : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(validationContext.ObjectInstance is PatchUpdateBase obj))
                return ValidationResult.Success;

            foreach (var item in obj.Origin)
            {
                if (item.Key.EqualsIgnoreCase(validationContext.MemberName))
                {
                    var errorResult = new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName),
                        new string[] {validationContext.MemberName});

                    // 存在该属性
                    return !base.IsValid(value) ? errorResult : ValidationResult.Success;
                }
            }

            // 不存在该属性,不验证
            return ValidationResult.Success;
        }
    }
}