using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity.DataProtection;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Lookup;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity
{
    public class IdentityConfigure : IAutowiredConfigureServices
    {
        public IdentityConfigure(IOptions<ShashlikIdentityOptions> options)
        {
            Options = options;
        }

        private IOptions<ShashlikIdentityOptions> Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            kernelService.Services.TryAddScoped<IUserConfirmation<Users>, DefaultUserConfirmation<Users>>();
            kernelService.Services.TryAddScoped<RoleManager<Roles>>();
            kernelService.Services.AddIdentityCore<Users>(options => { Options.Value.IdentityOptions.CopyTo(options); })
                .AddRoles<Roles>()
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                .AddRoleValidator<RoleValidator<Roles>>()
                .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
                ;

            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                {
                    Options.Value.DataProtectionTokenProviderOptions.CopyTo(o);
                }
            );
        }
    }
}