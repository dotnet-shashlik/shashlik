using System;
using System.ComponentModel.DataAnnotations;
using Guc.Utils.Extensions;
using System.Collections;
using Guc.Utils.Common;
using static Guc.Utils.Consts;

namespace Guc.Validation
{
    /// <summary>
    /// 是否为图片地址验证,需要注册IHttpContextAccessor
    /// </summary>
    public class ImgAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                try
                {
                    if (value.GetType() == typeof(string))
                    {
                        var url = value.ToString();
                        if (!IsImage(url))
                            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}图片地址不正确");
                    }
                    else if (value.GetType().IsChildTypeOf<IEnumerable>())
                    {
                        foreach (var item in (value as IEnumerable))
                        {
                            if (item == null)
                                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}图片地址不正确");
                            if (!IsImage(item.ToString()))
                                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}图片地址不正确");
                        }
                    }
                    else
                        throw new Exception($"{nameof(ImgAttribute)}只能用于{nameof(String)}或者{nameof(String)}集合属性");
                }
                catch
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}图片地址不正确");
                }
            }

            return ValidationResult.Success;
        }

        private bool IsImage(string url)
        {
            if (url.IsMatch(Regexs.Url))
            {
                return ImgHelper.IsImage(url);
            }
            else
                return false;
        }
    }
}
