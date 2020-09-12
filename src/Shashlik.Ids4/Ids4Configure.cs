using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

            if (Options.SignOptions.CredentialType == Ids4Options.CredentialType.dev)
                builder.AddDeveloperSigningCredential();
            else if (Options.SignOptions.CredentialType == Ids4Options.CredentialType.rsa)
            {
                if (Options.SignOptions.RsaPrivateKey.IsNullOrEmpty())
                    throw new ArgumentException($"Invalid rsa private key");
                var rsa = RSA.Create();
                rsa.ImportPrivateKey(Options.SignOptions.RsaKeyType, Options.SignOptions.RsaPrivateKey,
                    Options.SignOptions.RsaIsPem);
                builder.AddSigningCredential(new RsaSecurityKey(rsa), Options.SignOptions.SigningAlgorithm);
            }
            else if (Options.SignOptions.CredentialType == Ids4Options.CredentialType.x509)
            {
                X509Certificate2 certificate;
                if (!Options.SignOptions.X509CertificateContent.IsNullOrEmpty())
                {
                    var bytes = Convert.FromBase64String(Options.SignOptions.X509CertificateContent!);
                    certificate = new X509Certificate2(bytes, Options.SignOptions.X509CertificatePassword);
                }
                else if (!Options.SignOptions.X509CertificateFile.IsNullOrEmpty())
                {
                    if (!File.Exists(Options.SignOptions.X509CertificateFile))
                        throw new FileNotFoundException($"Cannot found certificate file.",
                            Options.SignOptions.X509CertificateFile);

                    certificate = new X509Certificate2(Options.SignOptions.X509CertificateFile!,
                        Options.SignOptions.X509CertificatePassword);
                }
                else
                    throw new InvalidOperationException("X509Certificate cannot be empty. ");


                builder.AddSigningCredential(certificate, Options.SignOptions.SigningAlgorithm);
            }
            else
                throw new InvalidOperationException($"Invalid credential type: {Options.SignOptions.CredentialType}");

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