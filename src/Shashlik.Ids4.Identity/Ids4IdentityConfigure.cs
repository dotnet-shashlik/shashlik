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
        public Ids4IdentityConfigure(IOptions<ShashlikIds4IdentityOptions> options)
        {
            Options = options.Value;
        }

        private ShashlikIds4IdentityOptions Options { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!Options.Enable)
                return;

            builder.AddAspNetIdentity<Users>();
            // 替换默认的密码认证器
            builder.Services.Replace(ServiceDescriptor
                .Transient<IResourceOwnerPasswordValidator, PasswordValidator>());

            // 验证码登录
            builder.AddExtensionGrantValidator<CaptchaValidator>();
            // 手机短信双因子验证码
            builder.AddExtensionGrantValidator<TwoFactorValidator>();
        }
    }
}