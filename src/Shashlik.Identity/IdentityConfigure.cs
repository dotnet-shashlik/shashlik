using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Captcha;
using Shashlik.Identity.DataProtection;
using Shashlik.Identity.Entities;
using Shashlik.Identity.Lookup;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity
{
    public class IdentityConfigure : IAutowiredConfigureServices
    {
        public IdentityConfigure(IOptions<ShashlikIdentityOptions> options, IOptions<CaptchaOptions> captchaOptions)
        {
            Options = options;
            CaptchaOptions = captchaOptions;
        }

        private IOptions<ShashlikIdentityOptions> Options { get; }
        private IOptions<CaptchaOptions> CaptchaOptions { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;

            var builder = kernelService.Services.AddIdentityCore<Users>(options =>
                {
                    Options.Value.IdentityOptions.CopyTo(options);
                })
                .AddRoles<Roles>()
                .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                .AddRoleValidator<RoleValidator<Roles>>()
                .AddPersonalDataProtection<DefaultLookupProtector, DefaultLookupProtectorKeyRing>()
                .AddDefaultTokenProviders();

            if (CaptchaOptions.Value.Enable)
                builder.AddTokenProvider<CaptchaTokenProvider>(ShashlikIdentityConsts.CaptchaTokenProvider);

            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                {
                    Options.Value.DataProtectionTokenProviderOptions.CopyTo(o);
                }
            );
        }
    }
}