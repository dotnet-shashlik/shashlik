using System;
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
        /// <exception cref="ArgumentException">参数异常</exception>
        /// <exception cref="Sms.Exceptions.SmsLimitException">发送频率限制异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        void SendCheck(string phone, string subject, params string[] args);

        /// <summary>
        /// 短信发送,会执行<see cref="SendCheck"/>
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="subject">验证码对应模板的类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        /// <exception cref="Sms.Exceptions.SmsServerException">短信服务端异常</exception>
        /// <exception cref="Sms.Exceptions.SmsLimitException">发送频率限制异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        /// <returns>此次短信发送的云厂商返回的请求id/业务id</returns>
        Task<string> SendWithCheckAsync(string phone, string subject, params string[] args);

        /// <summary>
        /// 批量短信发送(相同内容短信)
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="subject">业务短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        /// <exception cref="Sms.Exceptions.SmsServerException">短信服务端异常</exception>
        /// <exception cref="Sms.Exceptions.SmsTemplateException">模板配置异常</exception>
        /// <returns>此次短信发送的云厂商返回的请求id/业务id</returns>
        Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args);
    }
}