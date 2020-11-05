using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RSAExtensions;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4
{
    /// <summary>
    /// ids4自动配置,使用推荐的典型配置, 装配顺序800,可以通过<see cref="IIds4ExtensionAutowire"/>扩展配置
    /// </summary>
    [Order(800)]
    public class Ids4Autowire : IServiceAutowire
    {
        public Ids4Autowire(IOptions<Ids4Options> options)
        {
            Options = options.Value;
        }

        private Ids4Options Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;


            var builder = kernelService.Services
                .AddIdentityServer(options =>
                {
                    // 属性值复制
                    Options.IdentityServerOptions.CopyTo(options);
                });

            #region 签名证书

            // 使用开发证书,自动创建tempkey.rsa
            if (Options.SignOptions.CredentialType == CredentialType.Dev)
                builder.AddDeveloperSigningCredential();
            else if (Options.SignOptions.CredentialType == CredentialType.Rsa)
            {
                if (Options.SignOptions.RsaPrivateKey.IsNullOrEmpty())
                    throw new ArgumentException($"Invalid rsa private key");
                var rsa = RSA.Create();
                rsa.ImportPrivateKey(Options.SignOptions.RsaKeyType, Options.SignOptions.RsaPrivateKey,
                    Options.SignOptions.RsaIsPem);
                builder.AddSigningCredential(new RsaSecurityKey(rsa), Options.SignOptions.SigningAlgorithm);
            }
            else if (Options.SignOptions.CredentialType == CredentialType.X509)
            {
                X509Certificate2 certificate;
                if (!Options.SignOptions.X509CertificateFileContent.IsNullOrEmpty())
                {
                    var bytes = Convert.FromBase64String(Options.SignOptions.X509CertificateFileContent!);
                    if (Options.SignOptions.X509CertificatePassword.IsNullOrEmpty())
                        certificate = new X509Certificate2(bytes);
                    else
                        certificate = new X509Certificate2(bytes, Options.SignOptions.X509CertificatePassword);
                }
                else if (!Options.SignOptions.X509CertificateFilePath.IsNullOrEmpty())
                {
                    if (!File.Exists(Options.SignOptions.X509CertificateFilePath))
                        throw new FileNotFoundException($"Cannot found certificate file.",
                            Options.SignOptions.X509CertificateFilePath);

                    certificate = new X509Certificate2(Options.SignOptions.X509CertificateFilePath!,
                        Options.SignOptions.X509CertificatePassword);
                }
                else
                    throw new InvalidOperationException("X509Certificate cannot be empty. ");


                builder.AddSigningCredential(certificate, Options.SignOptions.SigningAlgorithm);
            }
            else
                throw new InvalidOperationException($"Invalid credential type: {Options.SignOptions.CredentialType}");

            #endregion

            #region client/resource

            // 配置内存数据: 客户端/资源
            if (Options.Clients != null)
                builder.AddInMemoryClients(Options.Clients);
            if (Options.IdentityResources != null)
                builder.AddInMemoryIdentityResources(Options.IdentityResources);
            if (Options.ApiResources != null)
                builder.AddInMemoryApiResources(Options.ApiResources);
            if (Options.ApiScopes != null)
                builder.AddInMemoryApiScopes(Options.ApiScopes);

            #endregion

            // 执行扩展的自定义配置
            kernelService.Autowire<IIds4ExtensionAutowire>(r => r.ConfigureIds4(builder));
        }
    }
}