using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Identity.AspNetCore;
using Shashlik.Identity.AspNetCore.Providers;
using Shashlik.Identity.Entities;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.Spa
{
    /// <summary>
    /// identity配置,集群环境下关于identity的token算法,强烈建议使用X509RsaDataProtector
    /// </summary>
    public class IdentityConfigure : IAutowiredConfigureServices
    {
        public IdentityConfigure(IOptions<ShashlikIdentityOptions> shashlikIdentityOptions,
            IOptions<ShashlikAspNetIdentityOptions> shashlikAspNetIdentityOptions)
        {
            ShashlikIdentityOptions = shashlikIdentityOptions.Value;
            ShashlikAspNetIdentityOptions = shashlikAspNetIdentityOptions.Value;
        }

        private ShashlikIdentityOptions ShashlikIdentityOptions { get; }
        private ShashlikAspNetIdentityOptions ShashlikAspNetIdentityOptions { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!ShashlikIdentityOptions.Enable)
                return;

            var builder = kernelService.Services
                    .AddIdentity<Users, Roles>(options =>
                    {
                        ShashlikAspNetIdentityOptions.IdentityOptions.CopyTo(options);
                    })
                    .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                    .AddDefaultTokenProviders()
                ;

            if (ShashlikAspNetIdentityOptions.UseCaptchaTokenProvider)
                // 注册验证码支持
                builder.AddTokenProvider<CaptchaTokenProvider>(ShashlikIdentityAspNetCoreConsts.CaptchaProvider);

            // 扩展identity配置
            kernelService.BeginAutowired<IIdentityBuilderConfigure>()
                .Build(r => (r.ServiceInstance as IIdentityBuilderConfigure)!.Configure(builder));
        }
    }
}