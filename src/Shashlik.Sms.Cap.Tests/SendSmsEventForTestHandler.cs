﻿using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shashlik.Cap;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Exceptions;

namespace Shashlik.Sms.Cap.Tests
{
    /// <summary>
    /// 发送短信事件,执行真正的短信发送
    /// </summary>
    [ConditionDependsOn(typeof(ISms))]
    public class SendSmsEventForTestHandler : IEventHandler<SendSmsEvent>
    {
        public SendSmsEventForTestHandler(ISms sms, ILogger<SendSmsEventForExecuteHandler> logger)
        {
            Sms = sms;
            Logger = logger;
        }

        private ISms Sms { get; }
        private ILogger<SendSmsEventForExecuteHandler> Logger { get; }

        public static bool HasSend { get; private set; }

        public async Task Execute(SendSmsEvent @event)
        {
            try
            {
                Sms.Send(@event.Phones, @event.Subject, @event.Args.ToArray());
            }
            catch (SmsDomainException e)
            {
                Logger.LogError(e, "sms send failed, domain error");
                throw;
            }
            catch (SmsOptionsException e)
            {
                Logger.LogError(e, "sms send failed, options error");
                throw;
            }

            await Task.CompletedTask;

            HasSend = true;
        }
    }
}