using System;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送,参数错误,一般不用重试了
    /// </summary>
    public class SmsArgException : Exception
    {
        public SmsArgException(string message) : base(message)
        {
        }

        public SmsArgException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
