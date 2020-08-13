using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Guc.Utils.Extensions;
using System.Collections;
using Guc.Utils.Common;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using static Guc.Utils.Consts;

namespace Guc.Validation
{
    /// <summary>
    /// 验证是否为合法的文件,需要注册IHttpContextAccessor
    /// </summary>
    public class FileAttribute : ValidationAttribute
    {
        IReadOnlyList<string> Extensions { get; }

        /// <summary>
        /// 文件格式验证
        /// </summary>
        /// <param name="extensions">文件扩展名,不区分大小写,不包含小数点</param>
        public FileAttribute(params string[] extensions)
        {
            Extensions = extensions.ToReadOnly();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                try
                {
                    if (value.GetType() == typeof(string))
                    {
                        var url = value.ToString();

                        var result = Val(url);
                        if (!result)
                            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}文件格式不正确");
                    }
                    else if (value.GetType().IsChildTypeOf<IEnumerable>())
                    {
                        foreach (var item in (value as IEnumerable))
                        {
                            if (item == null)
                                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}文件格式不正确");

                            var url = value.ToString();

                            var result = Val(url);
                            if (!result)
                                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}文件格式不正确");
                        }
                    }
                }
                catch
                {
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName ?? validationContext.MemberName}文件格式不正确");
                }
            }

            return ValidationResult.Success;
        }

        private bool Val(string url)
        {
            Stream stream;

            if (url.IsMatch(Regexs.Url))
            {
                stream = HttpHelper.GetStream(url).GetAwaiter().GetResult();
            }
            else
                return false;

            if (stream == null)
                return false;

            using (stream)
                return FileExtensionBytesMapping.IsMatch(stream, Extensions.ToArray());
        }
    }
}
