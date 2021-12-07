using System;
using System.Collections.Generic;
using Shashlik.Utils.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Shashlik.Sms.Exceptions
{
    public class SmsTemplateException : Exception
    {
        public IEnumerable<string> Phones { get; }
        public string Subject { get; }

        public SmsTemplateException(IEnumerable<string> phones, string smsSubject) : base(
            $"sms send to {phones.Join(",")} failed: subject of {smsSubject} configuration error")
        {
            Phones = phones;
            Subject = smsSubject;
        }
    }
}