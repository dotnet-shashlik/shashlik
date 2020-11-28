using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Exceptions;
using Shashlik.Sms.Options;
using Shashlik.Utils;

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

        public static void Valid(IEnumerable<string> phones, string subject, SmsOptions smsOptions, ISmsLimit smsLimit, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new SmsArgException("subject can't be empty");
            var enumerable = phones?.ToList();
            if (enumerable.IsNullOrEmpty())
                throw new SmsArgException($"phone number can't be empty");
            if (enumerable!.Count > smsOptions.BatchMax)
                throw new SmsArgException($"batch send max count: {smsOptions.BatchMax}");
            foreach (var phone in enumerable)
            {
                if (phone.IsNullOrWhiteSpace() || !phone.IsMatch(@"^[1][0-9]{10}$"))
                    throw new SmsArgException($"invalid phone number: {phone}");
            }

            var configs = smsOptions.DomainConfigs.OrderBy(r => r.Priority);
            foreach (var item in configs)
            {
                var template = item.Templates.FirstOrDefault(r => r.Subject.EqualsIgnoreCase(subject));
                if (template is null)
                    throw new SmsOptionsException($"not found sms subject: {item.Domain}.{subject}");
                if (template.TemplateId.IsNullOrWhiteSpace())
                    throw new SmsOptionsException($"template id is empty of {item.Domain}.{subject}");
            }

            if (enumerable.Count == 1 && !smsLimit.CanSend(enumerable[0], subject))
                throw new SmsLimitException("frequency limitation");
        }

        public void ValidSend(IEnumerable<string> phones, string subject, params string[] args)
        {
            Valid(phones, subject, SmsOptions.CurrentValue, SmsLimit, args);
        }

        public void Send(string phone, string subject, params string[] args)
        {
            Send(new[] {phone}, subject, args);
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            var list = phones.ToList();
            ValidSend(list, subject, args);

            var configs = SmsOptions.CurrentValue.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsDomain>().ToList();
            var success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms is null)
                        throw new SmsOptionsException($"not found sms domain: {item.Domain}");
                    try
                    {
                        domainSms.Send(item, list, subject, args);
                        success = true;
                        // 如果发送成功,退出循环
                        break;
                    }
                    catch (SmsDomainException ex)
                    {
                        // 这一类异常 自动使用下一个类型的主机发送短信
                        Logger.LogError(ex,
                            $"[{item.Domain}]send sms failed, will retry use next domain, phone: {list.Join(",")}, args: {args.Join(",")}");
                    }
                }
            }

            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"send sms failed, phone: {list.Join(",")}");

            try
            {
                if (list.Count == 1)
                    SmsLimit.SendDone(list[0], subject);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"SmsLimit.UpdateLimit error, phone: {list[0]}, subject: {subject}");
            }
        }
    }
}