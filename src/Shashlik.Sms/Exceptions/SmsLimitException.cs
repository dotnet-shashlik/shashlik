using System;

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信发送频率限制
    /// </summary>
    public class SmsLimitException : Exception
    {
        public SmsLimitException()
        {
        }

        public SmsLimitException(string? message, Exception innerException) : base(message, innerException)
        {
        }
    }
}