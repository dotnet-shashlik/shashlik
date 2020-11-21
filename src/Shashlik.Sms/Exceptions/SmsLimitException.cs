using System;

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信发送限制
    /// </summary>
    public class SmsLimitException : Exception
    {
        public SmsLimitException(string message) : base(message)
        {
        }

        public SmsLimitException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}