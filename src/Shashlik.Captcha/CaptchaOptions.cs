using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Captcha
{
    [AutoOptions("Shashlik.Captcha")]
    public class CaptchaOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 验证码过期时间,默认3分钟
        /// </summary>
        public int LifeTimeSecond { get; set; } = 3 * 60;

        /// <summary>
        /// 可以失败的次数,默认3次,totp无效
        /// </summary>
        public int MaxErrorCount { get; set; } = 3;
    }
}