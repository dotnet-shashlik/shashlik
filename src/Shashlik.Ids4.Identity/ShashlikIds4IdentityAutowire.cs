extern alias IdentityServer4AspNetIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Options;

namespace Shashlik.Ids4.Identity
{
    public class ShashlikIds4IdentityAutowire : IIds4ExtensionAutowire
    {
        public ShashlikIds4IdentityAutowire(IOptions<IdentityOptions> identityOptions,
            IOptions<ShashlikIds4IdentityOptions> shashlikIds4IdentityOptions,
            IOptions<IdentityUserExtendsOptions> identityUserExtendsOptions, IOptions<IdentityTypeOptions> identityTypeOptions)
        {
            IdentityTypeOptions = identityTypeOptions;
            IdentityOptions = identityOptions.Value;
            ShashlikIds4IdentityOptions = shashlikIds4IdentityOptions.Value;
            IdentityUserExtendsOptions = identityUserExtendsOptions.Value;
        }

        private IdentityOptions IdentityOptions { get; }
        private IdentityUserExtendsOptions IdentityUserExtendsOptions { get; }
        private ShashlikIds4IdentityOptions ShashlikIds4IdentityOptions { get; }
        private IOptions<IdentityTypeOptions> IdentityTypeOptions { get; }

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

            // invoke: builder.AddAspNetIdentity<Users>();
            {
                var method = typeof(IdentityServer4AspNetIdentity::Microsoft.Extensions.DependencyInjection.IdentityServerBuilderExtensions)
                    .GetMethod(nameof(IdentityServer4AspNetIdentity::Microsoft.Extensions.DependencyInjection.IdentityServerBuilderExtensions
                        .AddAspNetIdentity), new Type[] {typeof(IIdentityServerBuilder)});

                method!.MakeGenericMethod(IdentityTypeOptions.Value.UserType!)
                    .Invoke(null, new object?[] {builder});
            }

            // 移除默认的密码认证validator
            var serviceDescriptors = builder.Services.Where(r => r.ServiceType == typeof(IResourceOwnerPasswordValidator)).ToList();
            foreach (var serviceDescriptor in serviceDescriptors)
                builder.Services.Remove(serviceDescriptor);

            // 替换默认的密码认证器
            builder.Services.Add(ServiceDescriptor.Transient<IResourceOwnerPasswordValidator, ShashlikPasswordValidator>());

            // 验证码登录
            builder.AddExtensionGrantValidator<ShashlikCaptchaValidator>();
            // 手机短信双因子验证码
            builder.AddExtensionGrantValidator<ShashlikTwoFactorValidator>();
        }
    }
}