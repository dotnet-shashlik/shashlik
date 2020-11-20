using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.DataProtection;
using Shashlik.Identity.Lookup;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity
{
    /// <summary>
    /// identity自动装配,装配顺序700
    /// </summary>
    [Order(700)]
    public class IdentityAutowire : IServiceAutowire
    {
        public IdentityAutowire(IOptions<ShashlikIdentityOptions> options, IOptions<CaptchaOptions> captchaOptions,
            IOptions<OriginIdentityOptions> identityOptions)
        {
            Options = options;
            CaptchaOptions = captchaOptions;
            IdentityOptions = identityOptions;
        }

        private IOptions<ShashlikIdentityOptions> Options { get; }
        private IOptions<OriginIdentityOptions> IdentityOptions { get; }
        private IOptions<CaptchaOptions> CaptchaOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            kernelService.Services.AddDataProtection();

            // configure IdentityOptions
            kernelService.Services.Configure<IdentityOptions>(r => { IdentityOptions.Value.CopyTo(r); });

            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var identityTypeOptions = serviceProvider.GetRequiredService<IIdentityTypeConfigure>();
            if (identityTypeOptions is null)
                throw new IdentityTypeConfigureException();

            var type = identityTypeOptions.GetType()
                .GetAllInterfaces(false)
                .FirstOrDefault(r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IIdentityTypeConfigure<,,>));
            if (type is null)
                throw new IdentityTypeConfigureException();
            var userType = type.GetGenericArguments()[0];
            var roleType = type.GetGenericArguments()[1];
            var keyType = type.GetGenericArguments()[2];

            // registry ShashlikUserManager<,>
            kernelService.Services.AddScoped(typeof(ShashlikUserManager<,>).MakeGenericType(userType, keyType));

            // reflect AddIdentityCore
            var addIdentityCoreMethodInfo = typeof(IdentityServiceCollectionExtensions)
                .GetMethod(nameof(IdentityServiceCollectionExtensions.AddIdentityCore), new[] {typeof(IServiceCollection)});

            // registry identity core: AddIdentityCore<TUser>()
            var builder = addIdentityCoreMethodInfo!.MakeGenericMethod(userType)
                .Invoke(null, new object[] {kernelService.Services}) as IdentityBuilder;

            // registry role: AddRole<Role>()
            var addRoleMethodInfo = typeof(IdentityBuilder).GetMethod(nameof(IdentityBuilder.AddRoles), new Type[] { });
            addRoleMethodInfo!.MakeGenericMethod(roleType).Invoke(builder, new object[] { });

            builder!
                .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
                .AddDefaultTokenProviders();

            // registry CaptchaTokenProvider
            if (CaptchaOptions.Value.Enable)
            {
                var captchaTokenProviderType =
                    typeof(CaptchaTokenProvider<>).MakeGenericType(userType);

                builder.AddTokenProvider(ShashlikIdentityConsts.CaptchaTokenProvider, captchaTokenProviderType);
            }

            // configure DataProtectionTokenProviderOptions
            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(
                o => Options.Value.DataProtectionTokenProviderOptions.CopyTo(o));

            // extension identity
            kernelService.Autowire<IIdentityExtensionAutowire>(r => r.Configure(builder));
        }
    }
}