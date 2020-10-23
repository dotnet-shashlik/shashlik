using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
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
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.UseEmptySms", false)]
    class DefaultSms : ISms, ISingleton
    {
        public DefaultSms(IOptionsMonitor<SmsOptions> smsOptions, IServiceProvider serviceProvider,
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
            var invokerList = ServiceProvider.GetServices<ISmsDomain>().ToList();
            var success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms == null)
                        throw new SmsDomainException($"找不到对应的短信服务商接口:{item.Domain}");
                    try
                    {
                        domainSms.Send(item, new[] {phone}, subject, args);
                        success = true;
                        // 如果发送成功,退出循环
                        break;
                    }
                    catch (SmsDomainException ex)
                    {
                        // 这一类异常 自动使用下一个类型的主机发送短信
                        Logger.LogError(ex, $"[{item.Domain}]短信发送失败,phone:{phone}");
                    }
                }
            }

            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"短信发送失败,phone:{phone}");

            SmsLimit.UpdateLimit(phone, subject);
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can not be empty.");
            var enumerable = phones?.ToList();
            if (enumerable.IsNullOrEmpty())
                throw new SmsArgException("手机号码不能为空");
            if (enumerable!.Count() > SmsOptions.CurrentValue.PatchMax)
                throw new SmsArgException($"批量发送短信最多{SmsOptions.CurrentValue.PatchMax}个号码");

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsDomain>().ToList();
            var success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms == null)
                        throw new SmsDomainException($"找不到对应的短信服务商接口:{item.Domain}");
                    try
                    {
                        domainSms.Send(item, enumerable, subject, args);
                        success = true;
                        // 如果发送成功,退出循环
                        break;
                    }
                    catch (SmsDomainException ex)
                    {
                        // 这一类异常 自动使用下一个类型的主机发送短信
                        Logger.LogError(ex, $"[{item.Domain}]短信发送失败,phone:{enumerable.Join(",")}");
                    }
                }
            }

            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"短信发送失败,phone:{enumerable.Join(",")}");
        }
    }
}