using System.ComponentModel;

namespace Shashlik.Ids4.Identity
{
    /// <summary>
    /// 登录错误代码
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// 需要两步验证
        /// </summary>
        [Description("require two factor")] RequiresTwoFactor = 202,

        /// <summary>
        /// 用户不存在
        /// </summary>
        [Description("user not found")] UserNotFound = 400,

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        [Description("userName or password error")]
        UserNameOrPasswordError = 401,

        /// <summary>
        /// 用户已锁定
        /// </summary>
        [Description("user has been lockout")] UserLockout = 402,

        /// <summary>
        /// 不允许登录
        /// </summary>
        [Description("not allow login")] NotAllowLogin = 403,

        /// <summary>
        /// provider参数错误
        /// </summary>
        [Description("provider argument error")]
        ProviderError = 404,

        /// <summary>
        /// 验证码错误
        /// </summary>
        [Description("token argument error")] TokenError = 405,

        /// <summary>
        /// IdentityError
        /// </summary>
        [Description("identity argument error")]
        IdentityError = 406,

        /// <summary>
        /// SecurityError
        /// </summary>
        [Description("security argument error")]
        SecurityError = 407,

        /// <summary>
        /// 其他错误
        /// </summary>
        [Description("other error")] Other = 500
    }
}