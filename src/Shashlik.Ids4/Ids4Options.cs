using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Shashlik.Kernel.Autowire.Attributes;
using Shashlik.Utils.Rsa;

namespace Shashlik.Ids4
{
    [AutoOptions("Shashlik:Ids4")]
    public class Ids4Options
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = false;

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
            /// 是否使用开发证书,单机环境没问题,集群环境有问题或者使用相同的key文件
            /// </summary>
            public bool UseDevSigningCredential { get; set; } = false;

            /// <summary>
            /// rsa私钥(JWT token签名用), 优先使用该配置, 配置后UseDevSigningCredential无效,公钥自动导出
            /// </summary>
            public string? RsaPrivateKey { get; set; }

            /// <summary>
            /// 私钥类型,默认PKCS8,支持PKCS1/PKCS8/XML
            /// </summary>
            public RSAKeyType KeyType { get; set; } = RSAKeyType.Pkcs8;

            /// <summary>
            /// 私钥是不是pem格式,默认true
            /// </summary>
            public bool KeyIsPem { get; set; } = true;

            /// <summary>
            /// 签名算法,默认RS256
            /// </summary>
            public IdentityServerConstants.RsaSigningAlgorithm RsaSigningAlgorithm { get; set; } =
                IdentityServerConstants.RsaSigningAlgorithm.RS256;
        }
    }
}