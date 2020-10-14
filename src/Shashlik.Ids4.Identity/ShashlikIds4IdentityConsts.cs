using IdentityServer4.Models;

namespace Shashlik.Ids4.Identity //ShashlikIds4Identity
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class ShashlikIds4IdentityConsts
    {
        /// <summary>
        /// 通用验证码登录
        /// </summary>
        public const string CaptchaGrantType = "captcha";

        /// <summary>
        /// 手机双因子验证
        /// </summary>
        public const string TwoFactorGrantType = "twofactor";

        /// <summary>
        /// 密码登录
        /// </summary>
        public const string PasswordGrantType = GrantType.ResourceOwnerPassword;

        /// <summary>
        /// 用户名登录源
        /// </summary>
        public const string UsernameSource = "username";

        /// <summary>
        /// 手机号码登录源
        /// </summary>
        public const string PhoneSource = "phone";

        /// <summary>
        /// 邮件登录源
        /// </summary>
        public const string EMailSource = "email";

        /// <summary>
        /// 身份证号码登录源
        /// </summary>
        public const string IdCardSource = "idcard";

        /// <summary>
        /// 两阶段登录,数据加解密purpose
        /// </summary>
        public const string TwoFactorTokenProviderPurpose = "two_factor_purpose";
    }
}