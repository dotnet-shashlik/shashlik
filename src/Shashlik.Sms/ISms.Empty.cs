using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Inner;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 手机短信
    /// </summary>
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.UseEmptySms", true, DefaultValue = false)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    internal class EmptySms : ISms
    {
        public EmptySms(IOptionsMonitor<SmsOptions> smsOptions, ISmsLimit smsLimit)
        {
            SmsOptions = smsOptions;
            SmsLimit = smsLimit;
        }

        private IOptionsMonitor<SmsOptions> SmsOptions { get; }
        private ISmsLimit SmsLimit { get; }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can't be empty.");
            var enumerable = phones?.ToList();
            if (enumerable.IsNullOrEmpty())
                throw new SmsArgException($"phone number can't be empty.");
            if (enumerable!.Count > SmsOptions.CurrentValue.BatchMax)
                throw new SmsArgException($"batch send max count: {SmsOptions.CurrentValue.BatchMax}.");
            if (enumerable.Count == 1 && !SmsLimit.LimitCheck(enumerable[0], subject))
                throw new SmsLimitException("frequency limitation.");

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            foreach (var item in configs)
            {
                var template = item.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
                if (template is null)
                    throw new SmsDomainException($"not found sms domain: {item.Domain}");
            }

            if (enumerable.Count == 1)
                SmsLimit.UpdateLimit(enumerable[0], subject);
        }
    }
}