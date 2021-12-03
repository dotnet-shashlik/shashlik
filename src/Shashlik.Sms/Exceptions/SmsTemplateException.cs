using System;

namespace Shashlik.Sms.Exceptions
{
    public class SmsTemplateException : Exception
    {
        public string Subject { get; }

        public SmsTemplateException(string smsSubject, string message) : base(message)
        {
            Subject = smsSubject;
        }
    }
}
