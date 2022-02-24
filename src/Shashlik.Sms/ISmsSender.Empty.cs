using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 空短信发送
    /// </summary>
    [Singleton]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.UseEmptySms), true, DefaultValue = false)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    public class EmptySmsSender : AbstractSmsSender
    {
        public EmptySmsSender(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options, ILogger<EmptySmsSender> logger) : base(smsLimit, options)
        {
            Logger = logger;
        }

        protected ILogger<EmptySmsSender> Logger { get; }

        public override Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
        {
            Logger.LogInformation($"Empty sms send success, phone:{phones}, subject:{subject}.");
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}