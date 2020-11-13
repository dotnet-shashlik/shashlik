using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Cap;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shashlik.Sms.Cap
{
    [ConditionDependsOn(typeof(ISms))]
    [ConditionDependsOnMissing(typeof(ISmsSender))]
    public class CapSmsSender : ISmsSender, Kernel.Dependency.ISingleton
    {
        public CapSmsSender(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options, IEventPublisher eventPublisher)
        {
            SmsLimit = smsLimit;
            Options = options;
            EventPublisher = eventPublisher;
        }

        private ISmsLimit SmsLimit { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }
        private IEventPublisher EventPublisher { get; }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (phones == null) throw new ArgumentNullException(nameof(phones));
            var list = phones.ToList();
            if (list.Count > Options.CurrentValue.BatchMax)
                throw new SmsArgException($"批量发送短信最多{Options.CurrentValue.BatchMax}个号码");
            if (list.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Consts.Regexs.MobilePhoneNumber)))
                throw new SmsArgException($"{list.Join(",")} 存在手机号码格式错误");
            if (list.Count == 1 && !SmsLimit.LimitCheck(list.First(), subject))
                throw new SmsArgException("短信发送过于频繁");

            EventPublisher.Publish(new SendSmsEvent
            {
                Phones = list.ToList(),
                Subject = subject,
                Args = args.ToList()
            });
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] { phone }, subject, args);
        }
    }
}