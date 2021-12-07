using System;
using System.Collections.Generic;

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信发送 服务端异常
    /// </summary>
    public class SmsServerException : Exception
    {
        public IEnumerable<string> Phones { get; }

        /// <summary>
        /// 云厂商返回的错误代码
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// 短信请求返回数据
        /// </summary>
        public object? Response { get; }

        public SmsServerException(IEnumerable<string> phones, string message, string? errorCode, object? response) : base(message)
        {
            Phones = phones;
            ErrorCode = errorCode;
            Response = response;
        }

        public SmsServerException(IEnumerable<string> phones, string message, string? errorCode, object? response, Exception innerException) :
            base(message, innerException)
        {
            Phones = phones;
            ErrorCode = errorCode;
            Response = response;
        }
    }
}