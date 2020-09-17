using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Identity.AspNetCore.Providers;
using Shashlik.Identity.Entities;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.AspNetCore
{
    /// <summary>
    /// identity配置,集群环境下关于identity的token算法,强烈建议使用X509RsaDataProtector
    /// </summary>
    public class IdentityConfigure : IAutowiredConfigureServices
    {
        public IdentityConfigure(IOptions<ShashlikIdentityOptions> options,
            IOptions<ShashlikAspNetIdentityOptions> identityOptions)
        {
            Options = options.Value;
            IdentityOptions = identityOptions.Value;
        }

        private ShashlikIdentityOptions Options { get; }
        private ShashlikAspNetIdentityOptions IdentityOptions { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            var builder = kernelService.Services
                    .AddIdentity<Users, Roles>(options => { IdentityOptions.IdentityOptions.CopyTo(options); })
                    .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                    .AddDefaultTokenProviders()
                ;

            if (IdentityOptions.UseCaptchaToken)
            {
                builder.AddTokenProvider<EmailCaptchaProvider>(Consts.EmailCaptchaProvider);
                builder.AddTokenProvider<PhoneNumberCaptchaProvider>(IdentityOptions.IdentityOptions.Tokens
                    .ChangePhoneNumberTokenProvider);
            }

            // 扩展identity配置
            kernelService.BeginAutowired<IIdentityBuilderConfigure>()
                .Build(r => (r.ServiceInstance as IIdentityBuilderConfigure)!.Configure(builder));
        }
    }
}