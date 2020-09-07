using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.X509DataProtector
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
            if (Options.X509RsaPrivateKey.IsNullOrEmpty())
                throw new InvalidOperationException($"Cannot be empty private key.");

            var cer = new X509Certificate2(Encoding.UTF8.GetBytes(Options.X509RsaPrivateKey));
            if(!cer.HasPrivateKey)
                throw new InvalidOperationException($"Private key has been empty.");
                
            kernelService.Services.AddDataProtection()
                .ProtectKeysWithCertificate(cer);
        }
    }
}