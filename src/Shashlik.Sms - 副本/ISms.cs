using System.Collections.Generic;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信接口
    /// </summary>
    [Singleton]
    public interface ISms
    {
        /// <summary>
        /// 短信发送验证,参数/配置
        /// </summary>
        /// <param name="phones"></param>
        /// <param name="subject"></param>
        /// <param name="args"></param>
        void ValidSend(IEnumerable<string> phones, string subject, params string[] args);

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="phone">手机号码</param>
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