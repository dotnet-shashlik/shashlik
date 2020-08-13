using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Guc.Bank
{
    /// <summary>
    /// 银行编码验证
    /// </summary>
    public class BankAttribute : ValidationAttribute
    {
        public BankAttribute(string code)
        {
            Code = code;
        }

        /// <summary>
        /// 枚举名称
        /// </summary>
        public string Code { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value.GetType() != typeof(int))
                return ValidationResult.Success;

            var bankService = validationContext.GetRequiredService<IBankService>();
            if (bankService.All().Any(r => r.Code == Code))
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName} invalid enum value.");
        }
    }
}