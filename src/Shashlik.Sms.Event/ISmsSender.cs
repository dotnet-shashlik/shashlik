using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Utils.Extensions;
using static Shashlik.Utils.Consts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shashlik.Utils;
using Shashlik.EventBus;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Sms.Exceptions;
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

    [ConditionOnProperty("Shashlik.Sms.Enable", "true")]
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
            if (list.Count() > Options.CurrentValue.Max)
                throw new SmsArgException($"批量发送短信最多{Options.CurrentValue.Max}个号码");
            if (list.Any(m => m.IsNullOrWhiteSpace() || !m.IsMatch(Regexs.MobilePhoneNumber)))
                throw new SmsArgException($"{list.Join(",")} 存在手机号码格式错误");
            if (list.Count() == 1 && !SmsLimit.LimitCheck(list.First(), subject))
                throw new SmsArgException("短信发送过于频繁");

            // 先用异步直接发送一次短信,如果收到主机异常,再发布事件进行重试
            Task.Run(() =>
            {
                using var scope = Shashlik.Kernel.KernelServiceProvider.ServiceProvider.CreateScope();
                try
                {
                    var sms = scope.ServiceProvider.GetService<ISms>();
                    sms.Send(list, subject, args);
                }
                catch (SmsDomainException e)
                {
                    var eventPublisher = scope.ServiceProvider.GetService<IEventPublisher>();
                    eventPublisher.Publish(new SendSmsEvent
                    {
                        Phones = list.ToList(),
                        Subject = subject,
                        Args = args.ToList()
                    });
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetService<ILogger<ISmsSender>>();
                    logger.LogError(ex, $"短信发送异常:{list.Join(",")}");
                }
            });
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
        }
    }
}