using Shashlik.Kernel.Attributes;

namespace Shashlik.Captcha
{
    [AutoOptions("Shashlik.Captcha")]
    public class CaptchaOptions
    {
        public bool Enable { get; set; }
    }
}
