using Shashlik.Identity.DataProtection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Identity.Options
{
    [AutoOptions("Shashlik.Identity")]
    public class ShashlikIdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 验证码长度
        /// </summary>
        public int CaptchaLength { get; set; } = 6;

        /// <summary>
        /// 数据保护token配置项
        /// </summary>
        public DataProtectionTokenProviderOptions DataProtectionTokenProviderOptions { get; set; } =
            new DataProtectionTokenProviderOptions();
    }
}