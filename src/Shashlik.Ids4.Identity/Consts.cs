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
        /// 手机短信双因子验证
        /// </summary>
        public const string Phone2FAGrantType = "phone2fa";

        /// <summary>
        /// 邮件双因子验证
        /// </summary>
        public const string EMail2FAGrantType = "email2fa";

        /// <summary>
        /// 密码登录
        /// </summary>
        public const string PasswordGrantType = GrantType.ResourceOwnerPassword;

        /// <summary>
        /// 登录purpose
        /// </summary>
        public const string LoginPurpose = "login";
    }
}