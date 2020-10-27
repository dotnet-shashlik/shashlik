namespace Shashlik.Ids4.IdentityInt32
{
    /// <summary>
    /// 登录错误代码
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// 需要两步验证
        /// </summary>
        RequiresTwoFactor = 202,

        /// <summary>
        /// 用户不存在
        /// </summary>
        UserNotFound = 400,

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        UsernameOrPasswordError = 401,

        /// <summary>
        /// 用户已锁定
        /// </summary>
        UserLockout = 402,

        /// <summary>
        /// 不允许登录
        /// </summary>
        NotAllowLogin = 403,

        /// <summary>
        /// provider参数错误
        /// </summary>
        ProviderError = 404,

        /// <summary>
        /// 验证码错误
        /// </summary>
        TokenError = 405,

        /// <summary>
        /// IdentityError
        /// </summary>
        IdentityError = 406,

        /// <summary>
        /// SecurityError
        /// </summary>
        SecurityError = 407,

        /// <summary>
        /// SecurityTimeout
        /// </summary>
        SecurityTimeout = 408,

        /// <summary>
        /// 其他错误
        /// </summary>
        Other = 500
    }
}