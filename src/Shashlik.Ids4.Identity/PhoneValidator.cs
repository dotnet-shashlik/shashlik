using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 手机短信验证码验证码
    /// </summary>
    public class PhoneValidator : IExtensionGrantValidator
    {
        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            throw new System.NotImplementedException();
        }

        public string GrantType => Ids4IdentityGrantTypes.Phone;
    }
}