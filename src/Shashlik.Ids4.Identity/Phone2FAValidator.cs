using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机两部验证
    /// </summary>
    public class Phone2FAValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GrantType => Consts.Phone2FAGrantType;
    }
}