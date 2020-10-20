using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.EventBus;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Event
{
    [ConditionDependsOn(typeof(ISms))]
    public class DefaultSmsSender : ISmsSender, Kernel.Dependency.ISingleton
    {
        public DefaultSmsSender(ISmsLimit smsLimit, IOptionsMonitor<SmsOptions> options
        )
        {
            SmsLimit = smsLimit;
            Options = options;
        }

        private ISmsLimit SmsLimit { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (phones == null) throw new ArgumentNullException(nameof(phones));
            var list = phones?.ToList();
            if (list.Count() > Options.CurrentValue.PatchMax)
                throw new SmsArgException($"批量发送短信最多{Options.CurrentValue.PatchMax}个号码");
            if (list.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Consts.Regexs.MobilePhoneNumber)))
                throw new SmsArgException($"{list.Join(",")} 存在手机号码格式错误");
            if (list.Count() == 1 && !SmsLimit.LimitCheck(list.First(), subject))
                throw new SmsArgException("短信发送过于频繁");

            // 先用异步直接发送一次短信,如果收到主机异常,再发布事件进行重试
            Task.Run(() =>
            {
                using var scope = Kernel.KernelServiceProvider.ServiceProvider.CreateScope();
                try
                {
                    var sms = scope.ServiceProvider.GetService<ISms>();
                    sms.Send(list, subject, args);
                }
                catch (SmsArgException e)
                {
                    var logger = scope.ServiceProvider.GetService<ILogger<ISmsSender>>();
                    logger.LogError(e, $"短信发送异常:{list.Join(",")}");
                }
                catch (Exception e)
                {
                    scope.ServiceProvider.GetService<ILogger<DefaultSmsSender>>()
                        .LogError(e, $"短信发送异常, phone:{list.Join(",")}");
                    var eventPublisher = scope.ServiceProvider.GetService<IEventPublisher>();
                    eventPublisher.Publish(new SendSmsEvent
                    {
                        Phones = list.ToList(),
                        Subject = subject,
                        Args = args.ToList()
                    });
                }
            });
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
        }
    }
}