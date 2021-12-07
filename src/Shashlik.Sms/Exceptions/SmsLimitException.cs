using System;
using System.Collections.Generic;
using Shashlik.Utils.Extensions;

// ReSharper disable PossibleMultipleEnumeration

namespace Shashlik.Sms.Exceptions
{
    /// <summary>
    /// 短信发送频率限制
    /// </summary>
    public class SmsLimitException : Exception
    {
        private IEnumerable<string> Phones { get; }

        public SmsLimitException(IEnumerable<string> phones, Exception innerException) : base(
            $"sms send to {phones.Join(",")} failed: frequency limit. ",
            innerException)
        {
            Phones = phones;
        }

        public SmsLimitException(IEnumerable<string> phones) : base($"sms send to {phones.Join(",")} failed: frequency limit. ")
        {
            Phones = phones;
        }
    }
}