using System.ComponentModel.DataAnnotations;
using Guc.Utils.Common;
using Microsoft.Extensions.DependencyInjection;
using Guc.Utils.Extensions;

namespace Guc.Validation
{
    /// <summary>
    /// 小数精度验证
    /// </summary>
    public class PrecisionAttribute : ValidationAttribute
    {
        /// <summary>
        /// 小数精度验证
        /// </summary>
        /// <param name="precision">精度</param>
        public PrecisionAttribute(int precision)
        {
            Precision = precision;
        }

        /// <summary>
        /// 小数精度
        /// </summary>
        public int Precision { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            value.TryParse(out decimal v);
            if (v.IsMaxPrecision(Precision))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}最多保留2位小数");
        }
    }
}