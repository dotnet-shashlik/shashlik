﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.DataProtection;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Lookup;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity
{
    public class IdentityAutowire : IServiceAutowire
    {
        public IdentityAutowire(IOptions<ShashlikIdentityOptions> options, IOptions<CaptchaOptions> captchaOptions)
        {
            Options = options;
            CaptchaOptions = captchaOptions;
        }

        private IOptions<ShashlikIdentityOptions> Options { get; }
        private IOptions<CaptchaOptions> CaptchaOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            kernelService.Services.AddDataProtection();
            var builder = kernelService.Services.AddIdentityCore<Users>(options =>
                {
                    Options.Value.IdentityOptions.CopyTo(options);
                })
                .AddRoles<Roles>()
                .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
                .AddDefaultTokenProviders();

            if (CaptchaOptions.Value.Enable)
                builder.AddTokenProvider<CaptchaTokenProvider<Users>>(ShashlikIdentityConsts.CaptchaTokenProvider);
            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                {
                    Options.Value.DataProtectionTokenProviderOptions.CopyTo(o);
                }
            );

            kernelService.Autowire<IIdentityExtensionAutowire>(r => r.Configure(builder));
        }
    }
}