using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shashlik.Sms
{
    /// <summary>
    /// 手机短信
    /// </summary>
    class EmptySms : ISms
    {
        private readonly IOptionsSnapshot<SmsOptions> smsOptions;
        IServiceProvider ServiceProvider { get; }
        ILogger<DefaultSms> Logger { get; }
        IDistributedCache Cache { get; }


        public SmsOptions SmsOptions => smsOptions.Value;
        const string CachePrefix = "SMS_LIMIT:";
        const int OneDaySeconds = 60 * 60 * 24;

        public EmptySms(IOptionsSnapshot<SmsOptions> smsOptions, IServiceProvider serviceProvider, ILogger<DefaultSms> logger, IDistributedCache cache)
        {
            this.smsOptions = smsOptions;
            ServiceProvider = serviceProvider;
            Logger = logger;
            Cache = cache;
        }

        public void Send(string phone, string subject, params string[] args)
        {
            if (phone.IsNullOrWhiteSpace())
                throw new Exception($"手机号码不能为空");

            if (!phone.IsPhone())
                return;
            //throw new Exception($"存在非法的手机号码:[{phones.Join(",")}]");

            if (SmsOptions.DomainConfigs.IsNullOrEmpty())
                throw new Exception("短信配置为空");

            if (!LimitCheck(phone, subject))
                throw new Exception("短信发送频率过快");

            foreach (var item in SmsOptions.DomainConfigs)
            {
                var template = item.Templates.FirstOrDefault(r => r.SmsSubject.EqualsIgnoreCase(subject));
                if (template == null)
                    throw new Exception($"找不到短信类型{subject}");
                if (template.TemplateId.IsNullOrWhiteSpace())
                    throw new Exception($"{subject}未配置短信模板");
            }

            Logger.LogInformation($"发送空短信成功,phone:{phone},subject:{subject},args:{args?.Join(",")}");

            UpdateLimit(phone, subject);
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

        public void Send(IEnumerable<string> phone, string subject, params string[] args)
        {
        }
    }
}
