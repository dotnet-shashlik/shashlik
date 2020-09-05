using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Extensions;
using static Shashlik.Utils.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Utils;
using Shashlik.EventBus;
using Shashlik.Sms.Options;

namespace Shashlik.Sms.Event
{
    /// <summary>
    /// 短信发送器
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// 短信发送,批量发送,手机数量大于一个时,不会检查发送频率
        /// </summary>
        /// <param name="phones">手机号码</param>
        /// <param name="subject">短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        void Send(IEnumerable<string> phones, string subject, params string[] args);

        /// <summary>
        /// 短信发送,单个手机发送,会检查发送频率
        /// </summary>
        /// <param name="phone">手机号码</param>
        /// <param name="subject">短信类型</param>
        /// <param name="args">模板参数,注意参数顺序</param>
        void Send(string phone, string subject, params string[] args);
    }

    public class DefaultSmsSender : ISmsSender, Kernel.Dependency.ISingleton
    {
        public DefaultSmsSender(IEventPublisher eventPublisher,
            ILogger<DefaultSmsSender> logger, ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options)
        {
            EventPublisher = eventPublisher;
            Logger = logger;
            SmsLimit = smsLimit;
            Options = options;
        }

        private ILogger<DefaultSmsSender> Logger { get; }
        private IEventPublisher EventPublisher { get; }
        private ISmsLimit SmsLimit { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (phones == null) throw new ArgumentNullException(nameof(phones));
            var list = phones?.ToList();
            if (list.Count() > Options.CurrentValue.Max)
                throw new SmsArgException($"批量发送短信最多{Options.CurrentValue.Max}个号码");
            if (list.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Regexs.MobilePhoneNumber)))
                throw new SmsArgException($"{list.Join(",")} 存在手机号码格式错误");
            if (list.Count() == 1 && !SmsLimit.LimitCheck(list.First(), subject))
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
            if (phone.IsNullOrWhiteSpace() || !phone.IsMatch(Consts.Regexs.MobilePhoneNumber))
                throw new SmsArgException($"{phone} 手机号码格式错误");
            if (!SmsLimit.LimitCheck(phone, subject))
                throw new SmsArgException("短信发送过于频繁");

            EventPublisher.Publish(new SendSmsEvent
            {
                Phones = new List<string> {phone},
                Subject = subject,
                Args = args.ToList()
            });
        }
    }
}