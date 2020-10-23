using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Options;

namespace Shashlik.Ids4.Identity
{
    public class ShashlikIds4IdentityConfigure : IIds4ExtensionAutowire
    {
        public ShashlikIds4IdentityConfigure(IOptions<IdentityOptions> identityOptions,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions,
            IOptions<IdentityUserExtendsOptions> identityUserExtendsOptions)
        {
            IdentityOptions = identityOptions;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions;
            IdentityUserExtendsOptions = identityUserExtendsOptions;
        }

        private IOptions<IdentityOptions> IdentityOptions { get; }
        private IOptions<IdentityUserExtendsOptions> IdentityUserExtendsOptions { get; }
        private IOptions<ShashlikIds4IdentityOptions> ShashlikIds4IdentityOptions { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!ShashlikIds4IdentityOptions.Value.Enable)
                return;

            if (!IdentityUserExtendsOptions.Value.RequireUniqueIdCard
                && (ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .IdCardSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueIdCard = true"
                    });

            if (!IdentityUserExtendsOptions.Value.RequireUniqueIdCard
                && (ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .IdCardSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueIdCard = true"
                    });

            if (!IdentityUserExtendsOptions.Value.RequireUniquePhoneNumber
                && (ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniquePhoneNumber = true"
                    });

            if (!IdentityUserExtendsOptions.Value.RequireUniquePhoneNumber
                && (ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniquePhoneNumber = true"
                    });

            if (!IdentityOptions.Value.User.RequireUniqueEmail
                && (ShashlikIds4IdentityOptions.Value.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueEmail = true"
                    });

            if (!IdentityOptions.Value.User.RequireUniqueEmail
                && (ShashlikIds4IdentityOptions.Value.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource)))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueEmail = true"
                    });


            builder.AddAspNetIdentity<Users>();
            // 替换默认的密码认证器
            builder.Services.Replace(ServiceDescriptor
                .Transient<IResourceOwnerPasswordValidator, ShashlikPasswordValidator>());

            // 验证码登录
            builder.AddExtensionGrantValidator<ShashlikCaptchaValidator>();
            // 手机短信双因子验证码
            builder.AddExtensionGrantValidator<ShashlikTwoFactorValidator>();
        }
    }
}