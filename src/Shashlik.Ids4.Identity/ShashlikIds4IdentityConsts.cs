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

        /// <summary>
        /// 错误代码
        /// </summary>
        public class ErrorCodes
        {
            /// <summary>
            /// 用户名为空
            /// </summary>
            public const int UserNameEmpty = -400;

            /// <summary>
            /// 密码为空
            /// </summary>
            public const int PasswordEmpty = -401;

            /// <summary>
            /// 用户名或密码错误
            /// </summary>
            public const int UsernameOrPasswordError = -402;

            /// <summary>
            /// 手机号码错误
            /// </summary>
            public const int PhoneNumberError = -403;

            /// <summary>
            /// 邮件地址错误
            /// </summary>
            public const int EmailError = -404;

            /// <summary>
            /// provider参数错误
            /// </summary>
            public const int ProviderError = -405;

            /// <summary>
            /// 验证码错误
            /// </summary>
            public const int TokenError = -406;

            /// <summary>
            /// 用户已锁定
            /// </summary>
            public const int UserLockout = -407;

            /// <summary>
            /// 不允许登录
            /// </summary>
            public const int NotAllowLogin = -408;

            /// <summary>
            /// type参数错误
            /// </summary>
            public const int TypeError = -409;

            /// <summary>
            /// 用户不存在
            /// </summary>
            public const int UserNotFound = -410;

            /// <summary>
            /// IdentityError
            /// </summary>
            public const int IdentityError = -411;

            /// <summary>
            /// 其他错误
            /// </summary>
            public const int Other = -500;

            /// <summary>
            /// 需要两步验证
            /// </summary>
            public const int RequiresTwoFactor = -200;
        }
    }
}