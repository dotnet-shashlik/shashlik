using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms
{
    /// <summary>
    /// 抽象短信发送类
    /// </summary>
    public abstract class AbstractSmsProvider : ISmsProvider
    {
        public AbstractSmsProvider(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options)
        {
            SmsLimit = smsLimit;
            Options = options;
        }

        protected ISmsLimit SmsLimit { get; }
        protected IOptionsMonitor<SmsOptions> Options { get; }

        public abstract string ProviderName { get; }

        public virtual void SendCheck(string phone, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));

            var smsTemplates = Options.CurrentValue.Templates.GetOrDefault(subject);
            if (smsTemplates is null || smsTemplates.TemplateId.IsNullOrWhiteSpace() || smsTemplates.Sign.IsNullOrWhiteSpace())
            {
                throw new SmsTemplateException(new[] {phone}, subject);
            }

            if (!SmsLimit.CanSend(phone, subject))
            {
                throw new SmsLimitException(new[] {phone});
            }
        }

        public virtual Task<string> SendWithCheckAsync(string phone, string subject, params string[] args)
        {
            SendCheck(phone, subject, args);
            var requestId = SendAsync(new[] {phone}, subject, args);
            SmsLimit.SendDone(phone, subject);
            return requestId;
        }

        public abstract Task<string> SendAsync(IEnumerable<string> phones, string subject, params string[] args);
    }
}