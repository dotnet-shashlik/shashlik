using System;
using System.ComponentModel.DataAnnotations;
using Guc.Utils;
using Guc.Utils.Common;
using Guc.Utils.Extensions;

namespace Guc.Validation
{
    /// <summary>
    /// 枚举值验证
    /// </summary>
    public class Ipv4Attribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value.ToString().IsMatch(Consts.Regexs.Ipv4))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}格式错误");
        }
    }
}