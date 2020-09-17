using IdentityServer4.Models;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Consts
    {
        /// <summary>
        /// 手机短信验证
        /// </summary>
        public const string PhoneGrantType = "phone";

        /// <summary>
        /// 邮件验证
        /// </summary>
        public const string EMailGrantType = "email";

        /// <summary>
        /// 手机双因子验证
        /// </summary>
        public const string TwoFactorGrantType = "twofactor";

        /// <summary>
        /// 密码登录
        /// </summary>
        public const string PasswordGrantType = GrantType.ResourceOwnerPassword;

        /// <summary>
        /// 登录purpose
        /// </summary>
        public const string LoginPurpose = "login";

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
    }
}