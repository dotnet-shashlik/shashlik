using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送接口
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 短信发送前检查,参数/配置/发送频率
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="subject">验证码对应模板的类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        /// <returns>能否发送</returns>
        /// <exception cref="Sms.Exceptions.SmsLimitException">发送频率限制异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        void SendCheck(string phone, string subject, params string[] args);

        /// <summary>
        /// 验证码短信发送,会执行短信发送频率检查
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="subject">验证码对应模板的类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        /// <exception cref="Sms.Exceptions.SmsServerException">短信服务端异常</exception>
        /// <exception cref="Sms.Exceptions.SmsLimitException">发送频率限制异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        /// <returns>此次短信发送的请求id</returns>
        Task<string> SendWithLimitCheckAsync(string phone, string subject, params string[] args);

        /// <summary>
        /// 批量短信发送(相同内容短信)
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="subject">业务短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        /// <exception cref="Sms.Exceptions.SmsServerException">短信服务端异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        /// <returns>此次短信发送的请求id</returns>
        Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args);
    }
}