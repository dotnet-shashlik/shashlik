using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.DataProtector.X509
{
    /// <summary>
    /// 使用X509证书作为数据保护,主要用于分布式环境,默认是自动生成,在分布式/集群环境是有问题的
    /// </summary>
    public class X509RsaDataProtectorConfigure : IAutowiredConfigureServices
    {
        public X509RsaDataProtectorConfigure(IOptions<X509RsaDataProtectorOptions> options)
        {
            Options = options.Value;
        }

        private X509RsaDataProtectorOptions Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;
            if (Options.X509CertificateContent.IsNullOrEmpty() && Options.X509CertificateFile.IsNullOrEmpty())
                throw new InvalidOperationException($"Certificate cannot be empty.");
            X509Certificate2 certificate;
            if (!Options.X509CertificateContent.IsNullOrEmpty())
            {
                var bytes = Convert.FromBase64String(Options.X509CertificateContent);
                certificate = new X509Certificate2(bytes, Options.Password);
            }
            else
            {
                if (!File.Exists(Options.X509CertificateFile))
                    throw new FileNotFoundException("Certificate not found.", Options.X509CertificateFile);
                certificate = new X509Certificate2(Options.X509CertificateFile, Options.Password);
            }


            if (!certificate.HasPrivateKey)
                throw new InvalidOperationException($"Certificate must be contains private key.");

            kernelService.Services.AddDataProtection()
                // 禁用自动创建密钥
                .DisableAutomaticKeyGeneration()
                .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
                {
                    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                })
                // 设置应用名称
                .SetApplicationName(Options.ApplicationName)
                // 使用x509证书
                .ProtectKeysWithCertificate(certificate);

            kernelService.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromDays(Options.TokenLifespan));
        }
    }
}