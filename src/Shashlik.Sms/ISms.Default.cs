using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Guc.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace Guc.Sms
{
    /// <summary>
    /// 手机短信
    /// </summary>
    class DefaultSms : ISms
    {
        private readonly IOptionsSnapshot<SmsOptions> smsOptions;
        IServiceProvider ServiceProvider { get; }
        ILogger<DefaultSms> Logger { get; }
        IDistributedCache Cache { get; }

        public SmsOptions SmsOptions => smsOptions.Value;
        const string CachePrefix = "SMS_LIMIT:";
        const int OneDaySeconds = 60 * 60 * 24;

        public DefaultSms(IOptionsSnapshot<SmsOptions> smsOptions, IServiceProvider serviceProvider, ILogger<DefaultSms> logger, IDistributedCache cache)
        {
            this.smsOptions = smsOptions;
            ServiceProvider = serviceProvider;
            Logger = logger;
            Cache = cache;
        }

        public bool LimitCheck(string phone, string subject)
        {
            var limit = SmsOptions.Limits?.FirstOrDefault(r => r.Subject == subject);
            string key = CachePrefix + phone + "_" + subject;
            var now = DateTime.Now.GetLongDate();
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;

            if (limit != null && (limit.DayLimitCount.HasValue || limit.HourLimitCount.HasValue || limit.MinuteLimitCount.HasValue))
            {
                var smsLimit = Cache.GetObjectAsync<SmsLimit>(key).GetAwaiter().GetResult();
                if (smsLimit == null)
                    return true;

                if (smsLimit.Day != day)
                    return true;

                if (limit.DayLimitCount.HasValue && smsLimit.Records.Count >= limit.DayLimitCount)
                {
                    return false;
                }

                if (limit.HourLimitCount.HasValue && smsLimit.Records.Count(r => r.Hour == hour) >= limit.HourLimitCount)
                {
                    return false;
                }

                if (limit.MinuteLimitCount.HasValue && smsLimit.Records.Count(r => r.Hour == hour && r.Minute == minute) >= limit.MinuteLimitCount)
                {
                    return false;
                }
            }

            return true;
        }

        public void Send(string phone, string subject, params string[] args)
        {
            if (phone.IsNullOrWhiteSpace())
                throw new SmsArgException($"手机号码不能为空");
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new SmsArgException("subject can not be empty.");
            }

            if (!phone.IsPhone())
            {
                throw new SmsArgException("手机号码格式错误");
            }
            if (!LimitCheck(phone, subject))
                throw new SmsArgException("短信发送频率过快");

            var configs = SmsOptions.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsInvoker>();
            bool success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms == null)
                        throw new SmsDomainException($"找不到对应的短信服务商接口:{item.Domain}");
                    try
                    {
                        domainSms.Send(item, new[] { phone }, subject, args);
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

            UpdateLimit(phone, subject);
        }

        private void UpdateLimit(string phone, string subject)
        {
            var limit = SmsOptions.Limits?.FirstOrDefault(r => r.Subject == subject);
            if (limit == null)
                return;
            string key = CachePrefix + phone + "_" + subject;
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var second = DateTime.Now.Second;
            var expire = OneDaySeconds - hour * 60 * 60 - minute * 60 - second;

            var smsLimit = Cache.GetObjectAsync<SmsLimit>(key).GetAwaiter().GetResult();
            if (smsLimit == null)
                smsLimit = new SmsLimit
                {
                    Day = day,
                    Records = new List<SmsLimit.Record>()
                };
            smsLimit.Records.Add(new SmsLimit.Record
            {
                Hour = hour,
                Minute = minute
            });

            Cache.SetObjectAsync(key, smsLimit, expire).Wait();
        }

        public void Send(IEnumerable<string> phones, string subject, params string[] args)
        {
            if (phones.IsNullOrEmpty())
                throw new SmsArgException("手机号码不能为空");
            if (phones.Count() > 1000)
                throw new SmsArgException("批量发送短信最多发送1000条");

            var configs = SmsOptions.DomainConfigs.OrderBy(r => r.Priority);
            var invokerList = ServiceProvider.GetServices<ISmsInvoker>();
            bool success = false;
            foreach (var item in configs)
            {
                if (invokerList.Any(r => r.SmsDomain == item.Domain))
                {
                    var domainSms = invokerList.LastOrDefault(r => r.SmsDomain == item.Domain);
                    if (domainSms == null)
                        throw new SmsDomainException($"找不到对应的短信服务商接口:{item.Domain}");
                    try
                    {
                        domainSms.Send(item, phones, subject, args);
                        success = true;
                        // 如果发送成功,退出循环
                        break;
                    }
                    catch (SmsDomainException ex)
                    {
                        // 这一类异常 自动使用下一个类型的主机发送短信
                        Logger.LogError(ex, $"[{item.Domain}]短信发送失败,phone:{phones.Join(",")}");
                    }
                }
            }
            // 全部发送失败 则发送失败
            if (!success)
                throw new SmsDomainException($"短信发送失败,phone:{phones.Join(",")}");
        }
    }
}