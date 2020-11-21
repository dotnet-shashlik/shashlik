using System;

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信配置错误,一般需要重试
    /// </summary>
    public class SmsOptionsException : Exception
    {
        public SmsOptionsException(string message) : base(message)
        {
        }

        public SmsOptionsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}