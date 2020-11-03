using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
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
    /// <summary>
    /// identity自动装配,装配顺序700
    /// </summary>
    [Order(700)]
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

            // configure IdentityOptions
            kernelService.Services.Configure<IdentityOptions>(r => { Options.Value.IdentityOptions.CopyTo(r); });

            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var userRoleDefinition = serviceProvider.GetRequiredService<IIdentityUserRoleDefinition>();

            // registry ShashlikUserManager<,>
            kernelService.Services.AddScoped(typeof(ShashlikUserManager<,>).MakeGenericType(userRoleDefinition.UserType,
                GetKeyType(userRoleDefinition)));

            // reflect AddIdentityCore
            var addIdentityCoreMethodInfo = typeof(IdentityServiceCollectionExtensions)
                .GetMethod(nameof(IdentityServiceCollectionExtensions.AddIdentityCore),
                    new[] {typeof(IServiceCollection)});

            // registry identity core: AddIdentityCore<TUser>()
            var builder =
                addIdentityCoreMethodInfo!.MakeGenericMethod(userRoleDefinition.UserType)
                    .Invoke(null, new object[] {kernelService.Services}) as IdentityBuilder;

            // registry role: AddRole<Role>()
            var addRoleMethodInfo = typeof(IdentityBuilder).GetMethod(nameof(IdentityBuilder.AddRoles), new Type[] { });
            addRoleMethodInfo!.MakeGenericMethod(userRoleDefinition.RoleType).Invoke(builder, new object[] { });

            builder!
                .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
                .AddDefaultTokenProviders();

            // registry CaptchaTokenProvider
            if (CaptchaOptions.Value.Enable)
            {
                var captchaTokenProviderType =
                    typeof(CaptchaTokenProvider<>).MakeGenericType(userRoleDefinition.UserType);

                builder.AddTokenProvider(ShashlikIdentityConsts.CaptchaTokenProvider, captchaTokenProviderType);
            }

            // configure DataProtectionTokenProviderOptions
            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                {
                    Options.Value.DataProtectionTokenProviderOptions.CopyTo(o);
                }
            );

            // extension identity
            kernelService.Autowire<IIdentityExtensionAutowire>(r => r.Configure(builder));
        }

        private Type GetKeyType(IIdentityUserRoleDefinition userRoleDefinition)
        {
            if (!userRoleDefinition.UserType.IsSubTypeOfGenericType(typeof(IdentityUserBase<>))
                || userRoleDefinition.UserType.IsGenericTypeDefinition)
                throw new InvalidOperationException(
                    $"Error user type definition of {userRoleDefinition.UserType}, must implement from IdentityUsers<>.");
            if (!userRoleDefinition.RoleType.IsSubTypeOfGenericType(typeof(IdentityRoleBase<>))
                || userRoleDefinition.RoleType.IsGenericTypeDefinition)
                throw new InvalidOperationException(
                    $"Error role type definition of {userRoleDefinition.RoleType}, must implement from IdentityRoles<>.");

            var userKeyType = userRoleDefinition.UserType
                .GetAllBaseTypes()
                .FirstOrDefault(r => r.GetGenericTypeDefinition() == typeof(IdentityUser<>))
                ?.GetGenericArguments()
                .FirstOrDefault();

            if (userKeyType == null)
                throw new InvalidOperationException(
                    $"Error user type definition of {userRoleDefinition.UserType}, must implement from IdentityUsers<>.");

            var roleKeyType = userRoleDefinition.RoleType
                .GetAllBaseTypes()
                .FirstOrDefault(r => r.GetGenericTypeDefinition() == typeof(IdentityRole<>))
                ?.GetGenericArguments()
                .FirstOrDefault();

            if (roleKeyType == null)
                throw new InvalidOperationException(
                    $"Error role type definition of {userRoleDefinition.RoleType}, must implement from IdentityRoles<>.");


            if (userKeyType != roleKeyType)
                throw new InvalidOperationException("User key type must equals role key type.");

            return userKeyType;
        }
    }
}