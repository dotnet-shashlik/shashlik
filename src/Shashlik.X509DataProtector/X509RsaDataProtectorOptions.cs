using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.X509DataProtector
{
    [AutoOptions("Shashlik:X509DataProtector")]
    public class X509RsaDataProtectorOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// X509私钥,公钥自动导出
        /// </summary>
        public string X509RsaPrivateKey { get; set; }
    }
}