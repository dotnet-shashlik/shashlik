using Guc.Kernel.Dependency;
using System.Collections.Generic;

namespace Guc.Sms
{
    /// <summary>
    /// 短信接口
    /// </summary>
    public interface ISms
    {
        /// <summary>
        /// 发送频率限制检查
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        bool LimitCheck(string phone, string subject);

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="subject">短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        void Send(string phone, string subject, params string[] args);

        /// <summary>
        /// 批量短信发送,批量发送不会执行短信发送频率的检查,通知类短信适合用,验证码之类的最好不要用批量发送
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="subject">短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        void Send(IEnumerable<string> phones, string subject, params string[] args);
    }
}
