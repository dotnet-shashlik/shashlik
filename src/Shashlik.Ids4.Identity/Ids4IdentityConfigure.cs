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
        public Ids4IdentityConfigure(IOptions<Ids4IdentityOptions> options,
            IOptions<ShashlikIdentityOptions> identityOptions)
        {
            IdentityOptions = identityOptions.Value;
            Options = options.Value;
        }

        private Ids4IdentityOptions Options { get; }
        private ShashlikIdentityOptions IdentityOptions { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!Options.Enable)
                return;

            builder.AddAspNetIdentity<Users>();
            // 替换默认的密码认证器
            builder.Services.Replace(ServiceDescriptor
                .Transient<IResourceOwnerPasswordValidator, PasswordValidator>());
            if (IdentityOptions.UserProperty.PhoneNumberUnique)
            {
                // 手机短信验证码
                builder.AddExtensionGrantValidator<PhoneValidator>();
            }

            if (IdentityOptions.UserProperty.EmailUnique)
            {
                // 邮件验证码
                builder.AddExtensionGrantValidator<EmailValidator>();
            }

            // 手机短信双因子验证码
            builder.AddExtensionGrantValidator<Phone2FaValidator>();
        }
    }
}