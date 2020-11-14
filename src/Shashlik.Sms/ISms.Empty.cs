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
        public EmptySms(IOptionsMonitor<SmsOptions> smsOptions, IServiceProvider serviceProvider,
            ILogger<DefaultSms> logger, ISmsLimit smsLimit)
        {
            SmsOptions = smsOptions;
            ServiceProvider = serviceProvider;
            Logger = logger;
            SmsLimit = smsLimit;
        }

        private IOptionsMonitor<SmsOptions> SmsOptions { get; }
        private IServiceProvider ServiceProvider { get; }
        private ILogger<DefaultSms> Logger { get; }
        private ISmsLimit SmsLimit { get; }

        public void Send(string phone, string subject, params string[] args)
        {
            if (phone.IsNullOrWhiteSpace())
                throw new SmsArgException($"手机号码不能为空");
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can not be empty.");
            if (!phone.IsPhone())
                throw new SmsArgException("手机号码格式错误");
            if (!SmsLimit.LimitCheck(phone, subject))
                throw new SmsArgException("短信发送频率过快");

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            foreach (var item in configs)
            {
                var template = item.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
                if (template is null)
                    throw new Exception($"找不到短信类型{subject}");
                if (template.TemplateId.IsNullOrWhiteSpace())
                    throw new Exception($"{subject}未配置短信模板");
            }

            Logger.LogInformation($"发送空短信成功,phone:{phone},subject:{subject},args:{args?.Join(",")}");
            SmsLimit.UpdateLimit(phone, subject);
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can not be empty.");
            var enumerable = phones?.ToList();
            if (enumerable.IsNullOrEmpty())
                throw new SmsArgException("手机号码不能为空");
            if (enumerable!.Count() > 1000)
                throw new SmsArgException("批量发送短信最多发送1000条");

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            foreach (var item in configs)
            {
                var template = item.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
                if (template is null)
                    throw new Exception($"找不到短信类型{subject}");
                if (template.TemplateId.IsNullOrWhiteSpace())
                    throw new Exception($"{subject}未配置短信模板");
            }
        }
    }
}