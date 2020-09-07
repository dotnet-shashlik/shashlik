using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Shashlik.Utils.Rsa;

namespace Shashlik.Ids4
{
    /// <summary>
    /// ids4自动配置,使用推荐的典型配置, 可以通过<see cref="IIdentityServerBuilderConfigure"/>扩展配置
    /// </summary>
    public class Ids4Configure : IAutowiredConfigureServices, IAutowiredConfigureAspNetCore
    {
        public Ids4Configure(IOptions<Ids4Options> options)
        {
            Options = options.Value;
        }

        private Ids4Options Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            #region options

            var builder = kernelService.Services
                .AddIdentityServer(options =>
                {
                    // 属性值复制
                    Options.IdentityServerOptions.CopyTo(options);
                });

            #endregion

            #region 签名证书

            if (Options.SignOptions.UseDevSigningCredential && Options.SignOptions.RsaPrivateKey.IsNullOrEmpty())
                // 使用开发证书
                builder.AddDeveloperSigningCredential();
            if (!Options.SignOptions.RsaPrivateKey.IsNullOrEmpty())
            {
                var rsa = RSA.Create();
                rsa.ImportPrivateKey(Options.SignOptions.KeyType, Options.SignOptions.RsaPrivateKey,
                    Options.SignOptions.KeyIsPem);
                builder.AddSigningCredential(new RsaSecurityKey(rsa),
                    IdentityServerConstants.RsaSigningAlgorithm.RS256);
            }

            #endregion

            #region grant type

            // 注册自定义grantType
            var validators = AssemblyHelper.GetFinalSubTypes<IExtensionGrantValidator>();
            if (!validators.IsNullOrEmpty())
                validators.ForEach(r =>
                {
                    // 等于 builder.AddExtensionGrantValidator<T>
                    var des = ServiceDescriptor.Describe(typeof(IExtensionGrantValidator), r,
                        ServiceLifetime.Transient);
                    kernelService.Services.Add(des);
                });

            #endregion

            #region client/resource

            // 配置内存数据: 客户端/资源
            if (Options.Clients != null)
                builder.AddInMemoryClients(Options.Clients);
            if (Options.IdentityResources != null)
                builder.AddInMemoryIdentityResources(Options.IdentityResources);
            if (Options.ApiResources != null)
                builder.AddInMemoryApiResources(Options.ApiResources);

            #endregion

            #region profile

            var profiles = AssemblyHelper.GetFinalSubTypes<IProfileService>();
            if (!profiles.IsNullOrEmpty())
            {
                // 等于 builder.AddProfileService<>()
                var des = ServiceDescriptor.Describe(typeof(IProfileService), profiles.Single(),
                    ServiceLifetime.Transient);
                kernelService.Services.Add(des);
            }

            #endregion

            // 执行扩展的自定义配置
            kernelService
                .BeginAutowired<IIdentityServerBuilderConfigure>()
                .Build(r => (r.ServiceInstance as IIdentityServerBuilderConfigure)!.ConfigureIds4(builder));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}