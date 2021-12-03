using System.Threading.Tasks;

namespace Shashlik.Captcha
{
    /// <summary>
    /// 通用验证码功能,生成/验证(target可以是邮件/手机等 都可以,区分大小写)
    /// </summary>
    public interface ICaptcha
    {
        /// <summary>
        /// 自定义生成验证码数据,内部自动生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="lifeTimeSeconds">验证码有效时间，单位</param>
        /// <param name="maxErrorCount">最大错误次数</param>
        /// <param name="captchaLength">验证码长度</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等安全标识验证码需要失效</param>
        Task<string> Build(string purpose, string target, int lifeTimeSeconds = 300, int maxErrorCount = 3, int captchaLength = 6,
            string? securityStamp = null);

        /// <summary>
        /// 自定义生成验证码数据,调用方生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="lifeTimeSeconds">验证码有效时间，单位秒</param>
        /// <param name="maxErrorCount">最大错误次数</param>
        /// <param name="captcha">自行生成验证码并传入</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等安全标识验证码需要失效</param>
        /// <returns></returns>
        Task<string> Build(string purpose, string target, int lifeTimeSeconds, int maxErrorCount, string captcha, string? securityStamp = null);

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="captcha">验证码</param>
        /// <param name="securityStamp">target当前的安全标识,比如用户修改了密码等验证码需要失效</param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除</param>
        /// <returns></returns>
        Task<bool> IsValid(string purpose, string target, string captcha, string? securityStamp = null, bool isDeleteOnSucceed = true);
    }
}