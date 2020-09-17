using IdentityServer4.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Entities;

namespace Shashlik.Ids4.Identity
{
    public class Ids4IdentityConfigure : IIdentityServerBuilderConfigure
    {
        public Ids4IdentityConfigure(IOptions<Ids4IdentityOptions> options, IOptions<ShashlikIdentityOptions> identityOptions)
        {
            _identityOptions = identityOptions.Value;
            Options = options.Value;
        }

        private Ids4IdentityOptions Options { get; }
        private readonly ShashlikIdentityOptions _identityOptions;

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!Options.Enable)
                return;

            builder.AddAspNetIdentity<Users>();
            // 替换默认的密码认证器
            builder.Services.Replace(ServiceDescriptor
                .Transient<IResourceOwnerPasswordValidator, PasswordValidator<Users>>());
            if (_identityOptions.UserProperty.PhoneNumberUnique)
            {
                // 手机短信验证码
                builder.AddExtensionGrantValidator<PhoneValidator>();
            }

            if (_identityOptions.UserProperty.EMailUnique)
            {
                // 邮件验证码
                builder.AddExtensionGrantValidator<EMailValidator>();
            }
            
            // 手机短信验证码
            builder.AddExtensionGrantValidator<Phone2FAValidator>();
            // 邮件验证码
            builder.AddExtensionGrantValidator<EMail2FAValidator>();
        }
    }
}