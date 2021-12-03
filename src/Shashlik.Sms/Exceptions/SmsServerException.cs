using System;

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信发送 服务端异常
    /// </summary>
    public class SmsServerException : Exception
    {
        /// <summary>
        /// 云厂商返回的错误代码
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// 短信请求返回数据
        /// </summary>
        public object? Response { get; }

        public SmsServerException(string message, string? errorCode, object? response) : base(message)
        {
            ErrorCode = errorCode;
            Response = response;
        }

        public SmsServerException(string message, string? errorCode, object? response, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            Response = response;
        }
    }
}