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
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.UseEmptySms", false, DefaultValue = false)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    internal class DefaultSms : ISms
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
                throw new SmsArgException($"phone number can't be empty.");
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can't be empty.");
            if (!phone.IsPhone())
                throw new SmsArgException("invalid phone number.");
            if (!SmsLimit.LimitCheck(phone, subject))
                throw new SmsLimitException("frequency limitation.");

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsDomain>().ToList();
            var success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms is null)
                        throw new SmsDomainException($"not found sms domain: {item.Domain}");
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
                        Logger.LogError(ex, $"[{item.Domain}]send sms failed, will retry use next domain, phone: {phone}, args: {args.Join(",")}");
                    }
                }
            }

            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"send sms failed, phone: {phone}");

            SmsLimit.UpdateLimit(phone, subject);
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can't be empty.");
            var enumerable = phones?.ToList();
            if (enumerable.IsNullOrEmpty())
                throw new SmsArgException($"phone number can't be empty.");
            if (enumerable!.Count > SmsOptions.CurrentValue.BatchMax)
                throw new SmsArgException($"batch send max count: {SmsOptions.CurrentValue.BatchMax}.");
            if (enumerable.Count == 1 && !SmsLimit.LimitCheck(enumerable[0], subject))
                throw new SmsLimitException("frequency limitation.");


            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsDomain>().ToList();
            var success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms is null)
                        throw new SmsDomainException($"not found sms domain: {item.Domain}");
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
                        Logger.LogError(ex,
                            $"[{item.Domain}]send sms failed, will retry use next domain, phone: {enumerable.Join(",")}, args: {args.Join(",")}");
                    }
                }
            }

            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"send sms failed, phone: {enumerable.Join(",")}");

            if (enumerable.Count == 1)
                SmsLimit.UpdateLimit(enumerable[0], subject);
        }
    }
}