using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.Identity.HttpApi
{
    [AutoOptions("Shashlik.Identity.Api")]
    public class IdentityApiOptions
    {
        public bool Enable { get; set; }

        /// <summary>
        /// 是否允许匿名用户发送邮件验证码,默认false,一般用于注册即登录
        /// </summary>
        public bool AllowAnonymousEmailCaptcha { get; set; } = false;

        /// <summary>
        /// 是否允许匿名用户发送手机短信验证码,默认false,一般用于注册即登录
        /// </summary>
        public bool AllowAnonymousPhoneCaptcha { get; set; } = false;
    }
}