using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms
{
    /// <summary>
    /// 空短信发送
    /// </summary>
    [Singleton]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    public class EmptySmsProvider : AbstractSmsProvider
    {
        public EmptySmsProvider(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options, ILogger<EmptySmsProvider> logger) : base(smsLimit, options)
        {
            Logger = logger;
        }

        protected ILogger<EmptySmsProvider> Logger { get; }

        public override string ProviderName => Provider;

        public const string Provider = "empty";

        public override Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args)
        {
            Logger.LogInformation($"Empty sms send success, phone:{phones.Join(",")}, subject:{subject}.");
            return Task.FromResult(Guid.NewGuid().ToString());
        }
    }
}