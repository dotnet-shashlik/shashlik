using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Captcha
{
    [AutoOptions("Shashlik:Captcha")]
    public class CaptchaOptions
    {
        /// <summary>
        /// 验证码过期时间,默认60分钟
        /// </summary>
        public int ExpireSecond { get; set; } = 5 * 60;

        /// <summary>
        /// 可以失败的次数,默认3次
        /// </summary>
        public int MaxErrorCount { get; set; } = 3;
    }
}
