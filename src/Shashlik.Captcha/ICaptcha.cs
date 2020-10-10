using System.Threading.Tasks;

namespace Shashlik.Captcha
{
    /// <summary>
    /// 通用验证码功能,生成/验证(target可以是邮件/手机等 都可以,区分大小写)
    /// </summary>
    public interface ICaptcha
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="codeLength">验证码长度,totp无效</param>
        /// <returns></returns>
        Task<CodeModel> Build(string purpose, string target, int codeLength = 6);

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="purpose">验证类型,区分大小写</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,区分大小写</param>
        /// <param name="code"></param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除,totp无效</param>
        /// <returns></returns>
        Task<bool> IsValid(string purpose, string target, string code, bool isDeleteOnSucceed = true);
    }
}
