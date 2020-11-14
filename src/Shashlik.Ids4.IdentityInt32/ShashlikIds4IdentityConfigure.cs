using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity;
using Shashlik.Identity.Options;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4.IdentityInt32
{
    public class ShashlikIds4IdentityConfigure : IIds4ExtensionAutowire
    {
        public ShashlikIds4IdentityConfigure(IOptions<IdentityOptions> identityOptions,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions,
            IOptions<IdentityUserExtendsOptions> identityUserExtendsOptions)
        {
            IdentityOptions = identityOptions.Value;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions.Value;
            IdentityUserExtendsOptions = identityUserExtendsOptions.Value;
        }

        private IdentityOptions IdentityOptions { get; }
        private IdentityUserExtendsOptions IdentityUserExtendsOptions { get; }
        private ShashlikIds4IdentityOptions ShashlikIds4IdentityOptions { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (!ShashlikIds4IdentityOptions.Enable)
                return;

            if (ShashlikIds4IdentityOptions.PasswordSignInSources is null)
            {
                builder.Services.Configure<ShashlikIds4IdentityOptions>(r =>
                {
                    r.PasswordSignInSources = new List<string> {ShashlikIds4IdentityConsts.UsernameSource};
                });

                ShashlikIds4IdentityOptions.PasswordSignInSources =
                    new List<string> {ShashlikIds4IdentityConsts.UsernameSource};
            }

            if (ShashlikIds4IdentityOptions.CaptchaSignInSources is null)
            {
                builder.Services.Configure<ShashlikIds4IdentityOptions>(r =>
                {
                    r.CaptchaSignInSources = new List<string>
                        {ShashlikIds4IdentityConsts.PhoneSource, ShashlikIds4IdentityConsts.EMailSource};
                });
                ShashlikIds4IdentityOptions.CaptchaSignInSources = new List<string>
                    {ShashlikIds4IdentityConsts.PhoneSource, ShashlikIds4IdentityConsts.EMailSource};
            }


            if (!IdentityUserExtendsOptions.RequireUniqueIdCard
                && ShashlikIds4IdentityOptions.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .IdCardSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueIdCard = true"
                    });

            if (!IdentityUserExtendsOptions.RequireUniqueIdCard
                && ShashlikIds4IdentityOptions.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .IdCardSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[idcard] require unique idcard, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueIdCard = true"
                    });

            if (!IdentityUserExtendsOptions.RequireUniquePhoneNumber
                && ShashlikIds4IdentityOptions.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[phone] require unique phone, you should set Shashlik.Identity.IdentityOptions.User.RequireUniquePhoneNumber = true"
                    });

            if (!IdentityUserExtendsOptions.RequireUniquePhoneNumber
                && ShashlikIds4IdentityOptions.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .PhoneSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[phone] require unique phone, you should set Shashlik.Identity.IdentityOptions.User.RequireUniquePhoneNumber = true"
                    });

            if (!IdentityOptions.User.RequireUniqueEmail
                && ShashlikIds4IdentityOptions.PasswordSignInSources.Contains(ShashlikIds4IdentityConsts
                    .EMailSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "PasswordSignInSources[email] require unique email, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueEmail = true"
                    });

            if (!IdentityOptions.User.RequireUniqueEmail
                && ShashlikIds4IdentityOptions.CaptchaSignInSources.Contains(ShashlikIds4IdentityConsts
                    .EMailSource))
                throw new OptionsValidationException(nameof(ShashlikIds4IdentityOptions),
                    typeof(ShashlikIds4IdentityOptions),
                    new[]
                    {
                        "CaptchaSignInSources[email] require unique email, you should set Shashlik.Identity.IdentityOptions.User.RequireUniqueEmail = true"
                    });

            builder.AddAspNetIdentity<Users>();

            // 替换默认的密码认证器
            builder.Services.Replace(ServiceDescriptor
                .Transient<IResourceOwnerPasswordValidator, ShashlikPasswordValidator>());

            // 验证码登录
            builder.AddExtensionGrantValidator<ShashlikCaptchaValidator>();
            // 手机短信双因子验证码
            builder.AddExtensionGrantValidator<ShashlikTwoFactorValidator>();

            builder.Services.TryAddScoped<SignInManager<Users>>();
        }
    }
}