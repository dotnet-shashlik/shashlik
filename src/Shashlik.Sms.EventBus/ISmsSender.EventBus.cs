using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.EventBus;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.EventBus
{
    public class EventBusSmsSender : ISmsSender
    {
        public EventBusSmsSender(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options, IEventPublisher eventPublisher)
        {
            SmsLimit = smsLimit;
            Options = options;
            EventPublisher = eventPublisher;
        }

        private ISmsLimit SmsLimit { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }
        private IEventPublisher EventPublisher { get; }

        public void Send(IEnumerable<string> phones, string subject, TransactionContext transactionContext, params string[] args)
        {
            if (phones == null) throw new ArgumentNullException(nameof(phones));
            var list = phones.ToList();
            if (list.Count > Options.CurrentValue.PatchMax)
                throw new SmsArgException($"批量发送短信最多{Options.CurrentValue.PatchMax}个号码");
            if (list.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Consts.Regexs.MobilePhoneNumber)))
                throw new SmsArgException($"{list.Join(",")} 存在手机号码格式错误");
            if (list.Count == 1 && !SmsLimit.LimitCheck(list.First(), subject))
                throw new SmsArgException("短信发送过于频繁");
            EventPublisher.PublishAsync(new SendSmsEvent
            {
                Phones = list.ToList(),
                Subject = subject,
                Args = args.ToList()
            }, transactionContext).Wait();
            
        }

        public void Send(string phone, string subject, TransactionContext transactionContext, params string[] args)
        {
            Send(new[] {phone}, subject, transactionContext, args);
        }
    }
}
