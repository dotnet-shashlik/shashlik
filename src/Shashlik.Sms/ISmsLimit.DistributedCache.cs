using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shashlik.Sms.Inner;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms
{
    /// <summary>
    /// 分布式缓存短信发送限制
    /// </summary>
    public class DistributedCacheSmsLimit : ISmsLimit
    {
        public DistributedCacheSmsLimit(IDistributedCache cache, IOptionsMonitor<SmsOptions> options)
        {
            Cache = cache;
            Options = options;
        }

        private IDistributedCache Cache { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }

        // 0:phone,1:subject
        private const string CachePrefix = "SMS_LIMIT:{0}:{1}";

        public bool LimitCheck(string phone, string subject)
        {
            var limit = Options.CurrentValue.Limits?.FirstOrDefault(r => r.Subject == subject);
            var key = CachePrefix.Format(subject, phone);
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            if (limit != null && (limit.DayLimitCount.HasValue || limit.HourLimitCount.HasValue ||
                                  limit.MinuteLimitCount.HasValue))
            {
                var smsLimit = Cache.GetObjectAsync<SmsLimitModel>(key).GetAwaiter().GetResult();
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
            var expire = DateTimeOffset.Now.Date.AddDays(1);

            var smsLimit = Cache.GetObjectAsync<SmsLimitModel>(key).GetAwaiter().GetResult() ?? new SmsLimitModel
            {
                Day = day,
                Records = new List<SmsLimitModel.Record>()
            };
            smsLimit.Records.Add(new SmsLimitModel.Record
            {
                Hour = hour,
                Minute = minute
            });

            Cache.SetObjectAsync(key, smsLimit, expire).Wait();
        }
    }
}