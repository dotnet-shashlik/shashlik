﻿using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.DataProtector.X509
{
    [AutoOptions("Shashlik.DataProtector.X509")]
    public class X509RsaDataProtectorOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; } = "DefaultApplicationName";

        /**
         *    demo:
         *    var bytes = File.ReadAllBytes($@"./domain.pfx");
         *    var str = Convert.ToBase64String(bytes);
         */
        /// <summary>
        /// X509证书需要包含私钥,推荐pfx证书,证书文件内容,文件转成base64,优先使用此配置,适合放在配置文件中,比如远程配置
        /// </summary>
        public string X509CertificateContent { get; set; }

        /// <summary>
        /// X509证书需要包含私钥,推荐pfx证书,本地文件名,适合把证书放在本地
        /// </summary>
        public string X509CertificateFile { get; set; }

        /// <summary>
        /// 证书密码
        /// </summary>
        public string Password { get; set; }
    }
}