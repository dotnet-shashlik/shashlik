using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 邮件两步验证
    /// </summary>
    public class EMailT2FAValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GrantType => Consts.EMail2FAGrantType;
    }
}