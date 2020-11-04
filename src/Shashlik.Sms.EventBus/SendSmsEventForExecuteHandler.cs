using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shashlik.EventBus;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Exceptions;

namespace Shashlik.Sms.EventBus
{
    /// <summary>
    /// 发送短信事件,执行真正的短信发送
    /// </summary>
    [ConditionDependsOn(typeof(ISms))]
    public class SendSmsEventForExecuteHandler : IEventHandler<SendSmsEvent>
    {
        public SendSmsEventForExecuteHandler(ISms sms, ILogger<SendSmsEventForExecuteHandler> logger)
        {
            Sms = sms;
            Logger = logger;
        }

        private ISms Sms { get; }
        private ILogger<SendSmsEventForExecuteHandler> Logger { get; }

        public async Task Execute(SendSmsEvent @event, IDictionary<string, string> items)
        {
            await Task.CompletedTask;
            try
            {
                if (@event.Phones.Count == 1)
                    // 只有一个的时候使用单一手机号码发送
                    Sms.Send(@event.Phones[0], @event.Subject, @event.Args.ToArray());
                else
                    Sms.Send(@event.Phones, @event.Subject, @event.Args.ToArray());
            }
            catch (SmsArgException e)
            {
                Logger.LogError(e, "短信发送失败");
            }
            catch (SmsDomainException e)
            {
                Logger.LogError(e, "短信发送失败, 服务商错误");
                throw;
            }
        }
    }
}