using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

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
        /// PublicOrigin
        /// </summary>
        public string? PublicOrigin { get; set; }
        
        /// <summary>
        /// Base Path
        /// </summary>
        public  string? BasePath { get; set; }

        /// <summary>
        /// ids4配置参数
        /// </summary>
        public IdentityServerOptions IdentityServerOptions { get; set; } = new IdentityServerOptions();

        /// <summary>
        /// 签名配置,推荐使用rsa私钥配置,如果需要pfx等其他证书配置,自行扩展配置
        /// </summary>
        public SignOptions SignOptions { get; set; } = new SignOptions();

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
        /// api scopes 内存配置
        /// </summary>
        public List<ApiScope>? ApiScopes { get; set; }
    }
}