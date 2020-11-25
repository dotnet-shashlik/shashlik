using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils;

namespace Shashlik.Sms
{
    /// <summary>
    /// 手机短信
    /// </summary>
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.UseEmptySms", true, DefaultValue = false)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    internal class EmptySms : ISms
    {
        public EmptySms(IOptionsMonitor<SmsOptions> smsOptions, ISmsLimit smsLimit, ILogger<EmptySms> logger)
        {
            SmsOptions = smsOptions;
            SmsLimit = smsLimit;
            Logger = logger;
        }

        private IOptionsMonitor<SmsOptions> SmsOptions { get; }
        private ISmsLimit SmsLimit { get; }
        private ILogger<EmptySms> Logger { get; }

        public void ValidSend(IEnumerable<string> phones, string subject, params string[] args)
        {
            DefaultSms.Valid(phones, subject, SmsOptions.CurrentValue, SmsLimit, args);
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
            Logger.LogInformation($"Empty Sms Sent, phone:{phone}, subject:{subject}, args:{args.Join(",")}");
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            var list = phones.ToList();
            ValidSend(list, subject, args);
            try
            {
                if (list.Count == 1)
                    SmsLimit.SendDone(list[0], subject);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"SmsLimit.UpdateLimit error, phone: {list[0]}, subject: {subject}");
            }
        }
    }
}