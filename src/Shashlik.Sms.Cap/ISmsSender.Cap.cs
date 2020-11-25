using Shashlik.Cap;
using Shashlik.Kernel.Attributes;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Sms.Cap
{
    [ConditionDependsOn(typeof(ISms))]
    [Singleton]
    public class CapSmsSender : ISmsSender
    {
        public CapSmsSender(IEventPublisher eventPublisher, ISms sms)
        {
            EventPublisher = eventPublisher;
            Sms = sms;
        }

        private IEventPublisher EventPublisher { get; }
        private ISms Sms { get; }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            var list = phones as string[] ?? phones.ToArray();
            Sms.ValidSend(list, subject, args);
            EventPublisher.Publish(new SendSmsEvent
            {
                Phones = list.ToList(),
                Subject = subject,
                Args = args.ToList()
            });
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
        }
    }
}