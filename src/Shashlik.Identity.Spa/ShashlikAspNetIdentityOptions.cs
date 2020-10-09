using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Identity.Spa
{
    [AutoOptions("Shashlik.Identity")]
    public class ShashlikAspNetIdentityOptions
    {
        /// <summary>
        /// identity 原生配置
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; } = new IdentityOptions();

        /// <summary>
        /// 注册Shashlik.Captcha token提供类,默认true
        /// </summary>
        public bool UseCaptchaTokenProvider { get; set; } = true;

        /// <summary>
        /// 验证码长度,默认6
        /// </summary>
        public int CaptchaLength { get; set; } = 6;
    }
}