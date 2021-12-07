using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 空短信发送
    /// </summary>
    [Singleton]
    [ConditionDependsOnMissing(typeof(ISmsSender))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.UseEmptySms), true, DefaultValue = false)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    internal class EmptySms : ISmsSender
    {
        public EmptySms(ILogger<EmptySms> logger, ISmsLimit smsLimit)
        {
            Logger = logger;
            SmsLimit = smsLimit;
        }

        private ILogger<EmptySms> Logger { get; }
        private ISmsLimit SmsLimit { get; }

        public bool SendCaptchaLimitCheck(string phone)
        {
            return SmsLimit.CanSend(phone);
        }

        public Task<string> SendCaptchaAsync(string phone, string subject, params string[] args)
        {
            var requestId = SendAsync(phone, subject, args);
            SmsLimit.SendDone(phone);
            return requestId;
        }

        public async Task<string> SendAsync(string phone, string subject, params string[] args)
        {
            await SendAsync(new[] { phone }, subject, args);
            return Guid.NewGuid().ToString();
        }

        public Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
        {
            Logger.LogInformation($"Empty sms send success, phone:{phones}, subject:{subject}.");
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}