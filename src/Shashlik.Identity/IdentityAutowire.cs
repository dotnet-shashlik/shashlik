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

// ReSharper disable ConditionIsAlwaysTrueOrFalse

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
            kernelService.Services.Configure<IdentityOptions>(r => { IdentityOptions.Value.CopyTo(r, true); });

            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var identityTypeConfigure = serviceProvider.GetService<IIdentityTypeConfigure>();
            Type userType, roleType, keyType;
            if (identityTypeConfigure != null)
            {
                var type = identityTypeConfigure.GetType()
                    .GetAllInterfaces(false)
                    .FirstOrDefault(r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IIdentityTypeConfigure<,,>));
                if (type is null)
                    throw new IdentityTypeConfigureException();
                userType = type.GetGenericArguments()[0];
                roleType = type.GetGenericArguments()[1];
                keyType = type.GetGenericArguments()[2];

                kernelService.Services.Configure<IdentityTypeOptions>(r =>
                {
                    r.UserType = userType;
                    r.RoleType = roleType;
                    r.KeyType = keyType;
                });
            }
            else
            {
                var identityTypeOptions = serviceProvider.GetRequiredService<IOptions<IdentityTypeOptions>>();
                if (identityTypeOptions.Value.UserType is null
                    || identityTypeOptions.Value.RoleType is null
                    || identityTypeOptions.Value.KeyType is null)
                    throw new OptionsValidationException(nameof(IdentityTypeOptions), typeof(IdentityTypeOptions), new string[]
                    {
                        $"Make sure configure \"{nameof(IdentityTypeOptions)}\" and can't be null"
                    });

                userType = identityTypeOptions.Value.UserType;
                roleType = identityTypeOptions.Value.RoleType;
                keyType = identityTypeOptions.Value.KeyType;
            }

            // registry ShashlikUserManager<,>
            kernelService.Services.AddScoped(typeof(ShashlikUserManager<,>).MakeGenericType(userType, keyType));
            kernelService.Services.AddScoped(
                typeof(IShashlikUserManager),
                typeof(ShashlikUserManager<,>).MakeGenericType(userType, keyType));

            // registry ShashlikRoleManager<,>
            kernelService.Services.AddScoped(typeof(ShashlikRoleManager<,>).MakeGenericType(roleType, keyType));
            kernelService.Services.AddScoped(
                typeof(IShashlikRoleManager),
                typeof(ShashlikRoleManager<,>).MakeGenericType(roleType, keyType));

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