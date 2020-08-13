using Guc.Features.VerifyCode;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Features.VerifyCode
{
    /// <summary>
    /// 通用验证码功能,生成/验证(target可以是邮件/手机等 都可以,不区分大小写)
    /// </summary>
    public interface IVerifyCodeFeature : Guc.Kernel.Dependency.ITransient
    {
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <param name="subject">验证类型</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,不区分大小写</param>
        /// <param name="codeLength">验证码长度</param>
        /// <returns></returns>
        Task<Codes> Build(string subject, string target, int codeLength = 6);

        /// <summary>
        /// 验证码是否正确
        /// </summary>
        /// <param name="subject">验证类型</param>
        /// <param name="target">验证对象,target可以是邮件/手机等 都可以,不区分大小写</param>
        /// <param name="code"></param>
        /// <param name="isDeleteOnSucceed">验证成功后是否删除</param>
        /// <returns></returns>
        Task<bool> IsValid(string subject, string target, string code, bool isDeleteOnSucceed = true);
    }
}
