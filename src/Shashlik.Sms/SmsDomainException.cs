using System;

namespace Shashlik.Sms
{
    /// <summary>
    /// 短信发送 主机异常
    /// </summary>
    public class SmsDomainException : Exception
    {
        public SmsDomainException(string message) : base(message)
        {
        }

        public SmsDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
