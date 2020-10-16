using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using RSAExtensions;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Ids4
{
    [AutoOptions("Shashlik.Ids4")]
    public class Ids4Options
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// ids4配置参数
        /// </summary>
        public IdentityServerOptions IdentityServerOptions { get; set; } = new IdentityServerOptions();

        /// <summary>
        /// 签名配置,推荐使用rsa私钥配置,如果需要pfx等其他证书配置,自行扩展配置
        /// </summary>
        public _SignOptions SignOptions { get; set; } = new _SignOptions();

        /// <summary>
        /// 客户端数据,内存配置
        /// </summary>
        public List<Client>? Clients { get; set; }

        /// <summary>
        /// identity resources 内存配置
        /// </summary>
        public List<IdentityResource>? IdentityResources { get; set; }

        /// <summary>
        /// api resources 内存配置
        /// </summary>
        public List<ApiResource>? ApiResources { get; set; }

        /// <summary>
        /// 签名配置
        /// </summary>
        public class _SignOptions
        {
            /// <summary>
            /// 签名证书类型,不区分大小写,dev/rsa/x509,默认dev,dev配置存在集群问题,最好使用rsa或者x509
            /// </summary>
            public CredentialType CredentialType { get; set; } = CredentialType.dev;

            /// <summary>
            /// 签名算法,RS256,rsa/x509证书配置有效
            /// </summary>
            public string? SigningAlgorithm { get; set; } = "RS256";

            /// <summary>
            /// rsa: rsa私钥(JWT token签名用),  配置后UseDevSigningCredential无效,公钥自动导出
            /// </summary>
            public string? RsaPrivateKey { get; set; }

            /// <summary>
            /// rsa: 私钥类型,默认PKCS8,支持PKCS1/PKCS8/XML
            /// </summary>
            public RSAKeyType RsaKeyType { get; set; } = RSAKeyType.Pkcs8;

            /// <summary>
            /// rsa: 私钥是不是pem格式,默认true
            /// </summary>
            public bool RsaIsPem { get; set; } = true;

            /// <summary>
            /// x509: x509证书内容,文件base64后,一般pfx格式,和<see cref="X509CertificateFilePath"/>选择配置一个就可以,优先使用此配置
            /// </summary>
            public string? X509CertificateFileContent { get; set; }

            /// <summary>
            /// x509: 证书文件地址,和<see cref="X509CertificateFileContent"/>选择配置一个就可以
            /// </summary>
            public string? X509CertificateFilePath { get; set; }

            /// <summary>
            /// x509: 证书密码
            /// </summary>
            public string? X509CertificatePassword { get; set; }
        }

        public enum CredentialType
        {
            dev = 1,
            rsa = 2,
            x509 = 3
        }
    }
}