using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            IOptions<ShashlikAspNetIdentityOptions> options2)
        {
            Options1 = options.Value;
            Options2 = options2.Value;
        }

        private ShashlikIdentityOptions Options1 { get; }
        private ShashlikAspNetIdentityOptions Options2 { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options1.Enable)
                return;

            if (Options1.UseBCryptPasswordHasher)
                // 使用bcrypt作为密码hash算法
                kernelService.Services.AddScoped<IPasswordHasher<Users>, BCryptPasswordHasher<Users>>();

            var builder = kernelService.Services
                    .AddIdentity<Users, Roles>(options => Options2.IdentityOptions.CopyTo(options))
                    .AddEntityFrameworkStores<ShashlikIdentityDbContext>()
                    .AddDefaultTokenProviders()
                ;

            kernelService.BeginAutowired<IIdentityBuilderConfigure>()
                .Build(r => (r.ServiceInstance as IIdentityBuilderConfigure)!.Configure(builder));
        }
    }
}