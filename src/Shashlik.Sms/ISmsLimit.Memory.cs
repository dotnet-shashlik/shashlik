using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms
{
    /// <summary>
    /// 内存缓存短信发送限制
    /// </summary>
    public class MemorySmsLimit : ISmsLimit
    {
        public MemorySmsLimit(IMemoryCache cache, IOptionsMonitor<SmsOptions> options)
        {
            Cache = cache;
            Options = options;
        }

        private IMemoryCache Cache { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }

        // 0:phone,1:subject
        private const string CachePrefix = "SMS_LIMIT:{0}:{1}";

        public bool LimitCheck(string phone, string subject)
        {
            var limit = Options.CurrentValue.Limits?.FirstOrDefault(r => r.Subject == subject);
            string key = CachePrefix.Format(subject, phone);
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            if (limit != null && (limit.DayLimitCount.HasValue || limit.HourLimitCount.HasValue ||
                                  limit.MinuteLimitCount.HasValue))
            {
                var smsLimit = Cache.Get<SmsLimitModel>(key);
                if (smsLimit == null)
                    return true;

                if (smsLimit.Day != day)
                    return true;

                if (limit.DayLimitCount.HasValue && smsLimit.Records.Count >= limit.DayLimitCount)
                {
                    return false;
                }

                if (limit.HourLimitCount.HasValue &&
                    smsLimit.Records.Count(r => r.Hour == hour) >= limit.HourLimitCount)
                {
                    return false;
                }

                if (limit.MinuteLimitCount.HasValue &&
                    smsLimit.Records.Count(r => r.Hour == hour && r.Minute == minute) >= limit.MinuteLimitCount)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateLimit(string phone, string subject)
        {
            var limit = Options.CurrentValue.Limits?.FirstOrDefault(r => r.Subject == subject);
            if (limit == null)
                return;
            var key = CachePrefix.Format(subject, phone);
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var smsLimit = Cache.Get<SmsLimitModel>(key) ?? new SmsLimitModel
            {
                Day = day,
                Records = new List<SmsLimitModel.Record>()
            };

            smsLimit.Records.Add(new SmsLimitModel.Record
            {
                Hour = hour,
                Minute = minute
            });

            Cache.Set(key, smsLimit, DateTimeOffset.Now.Date.AddDays(1));
        }
    }
}