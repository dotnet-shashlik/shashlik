using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.Extensions.Localization;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 邮件验证码验证器
    /// </summary>
    public class EMailValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GrantType => Consts.EMailGrantType;
    }
}