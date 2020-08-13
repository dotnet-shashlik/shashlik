using System.Collections.Generic;
using System.Linq;
using Guc.Utils.Extensions;
using static Guc.Utils.Consts;
using Microsoft.Extensions.Logging;
using Guc.Utils;
using Guc.Kernel.Exception;
using Guc.EventBus;

namespace Guc.Sms.Event
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

    public class DefaultSmsSender : ISmsSender, Guc.Kernel.Dependency.ITransient
    {
        public DefaultSmsSender(IEventPublisher eventPublisher, ILogger<DefaultSmsSender> logger, ISms sms)
        {
            EventPublisher = eventPublisher;
            Logger = logger;
            Sms = sms;
        }
        ILogger<DefaultSmsSender> Logger { get; }

        IEventPublisher EventPublisher { get; }

        ISms Sms { get; }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (phones != null && phones.Count() > 1000)
            {
                Logger.LogError($"批量短信发送最多1000条");
                return;
            }
            if (phones == null || phones.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Regexs.MobilePhoneNumber)))
            {
                Logger.LogError($"{phones.Join(",")}短信发送手机验证失败");
                return;
            }
            if (phones.Count() == 1 && !Sms.LimitCheck(phones.First(), subject))
                throw GucException.LogicalError("操作过于频繁");

            EventPublisher.Publish(new SendSmsEvent
            {
                Phones = phones.ToList(),
                Subject = subject,
                Args = args.ToList()
            });
        }

        public void Send(string phone, string subject, params string[] args)
        {
            if (phone.IsNullOrWhiteSpace() || !phone.IsMatch(Consts.Regexs.MobilePhoneNumber))
            {
                Logger.LogError($"{phone}短信发送手机验证失败");
                return;
            }

            if (!Sms.LimitCheck(phone, subject))
                throw GucException.LogicalError("操作过于频繁");

            EventPublisher.Publish(new SendSmsEvent
            {
                Phones = new List<string> { phone },
                Subject = subject,
                Args = args.ToList()
            });
        }
    }
}
