using System.ComponentModel.DataAnnotations;
using Shashlik.Utils.Common;
using System.Collections;
using System.Collections.Generic;

namespace Shashlik.Validation
{
    /// <summary>
    /// 字符串集合中的字符串长度验证
    /// </summary>
    public class ListStringLengthAttribute : ValidationAttribute
    {
        public ListStringLengthAttribute(int maximumLength)
        {
            MaximumLength = maximumLength;
        }

        public int MaximumLength { get; }

        public int MinimumLength { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var list = value as IEnumerable<string>;
            if (list == null)
                return ValidationResult.Success;

            foreach (var item in list)
            {
                if (item.Length > MaximumLength)
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName} 最多{MaximumLength}个字符");
                if (item.Length < MinimumLength)
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName} 最少{MinimumLength}个字符");
            }

            return ValidationResult.Success;
        }
    }
}