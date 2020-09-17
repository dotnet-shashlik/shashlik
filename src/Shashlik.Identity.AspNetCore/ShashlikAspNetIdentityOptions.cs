using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Identity.AspNetCore
{
    [AutoOptions("Shashlik.Identity")]
    public class ShashlikAspNetIdentityOptions
    {
        /// <summary>
        /// identity 原生配置
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; } = new IdentityOptions();

        /// <summary>
        /// 使用Shashlik.Captcha作为手机短信/邮件验证码的token,默认true
        /// </summary>
        public bool UseCaptchaToken { get; set; } = true;

        /// <summary>
        /// 验证码长度,默认6
        /// </summary>
        public int CaptchaLength { get; set; } = 6;
    }
}