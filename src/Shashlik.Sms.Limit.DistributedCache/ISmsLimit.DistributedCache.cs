using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Limit.DistributedCache
{
    /// <summary>
    /// 分布式缓存短信发送限制
    /// </summary>
    [ConditionDependsOn(typeof(IDistributedCache))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.EnableDistributedCacheLimit", true, DefaultValue = true)]
    public class DistributedCacheSmsLimit : ISmsLimit
    {
        public DistributedCacheSmsLimit(IDistributedCache cache, IOptionsMonitor<SmsOptions> options, ILock locker)
        {
            Cache = cache;
            Options = options;
            Locker = locker;
        }

        private IDistributedCache Cache { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }
        private ILock Locker { get; }

        // 0:phone,1:subject
        private const string CachePrefix = "SMS_LIMIT:{0}:{1}";

        public bool CanSend(string phone, string subject)
        {
            var limit = Options.CurrentValue.Limits.FirstOrDefault(r => r.Subject == subject);
            if (limit is null)
                return true;
            var key = CachePrefix.Format(subject, phone);
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var smsLimit = Cache.GetObjectWithJson<SmsLimitModel>(key);
            if (smsLimit is null)
                return true;

            if (smsLimit.Day != day)
                return true;
            if (smsLimit.Records.Count >= limit.DayLimitCount)
                return false;
            if (smsLimit.Records.Count(r => r.Hour == hour) >= limit.HourLimitCount)
                return false;
            if (smsLimit.Records.Count(r => r.Hour == hour && r.Minute == minute) >= limit.MinuteLimitCount)
                return false;

            return true;
        }

        public void SendDone(string phone, string subject)
        {
            var limit = Options.CurrentValue.Limits.FirstOrDefault(r => r.Subject == subject);
            if (limit is null)
                return;
            var key = CachePrefix.Format(subject, phone);
            using var @lock = Locker.Lock(key, 3);
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var expire = DateTimeOffset.Now.Date.AddDays(1);

            var smsLimit = Cache.GetObjectWithJson<SmsLimitModel>(key) ?? new SmsLimitModel
            {
                Day = day,
                Records = new List<SmsLimitModel.Record>()
            };
            smsLimit.Records.Add(new SmsLimitModel.Record
            {
                Hour = hour,
                Minute = minute
            });

            Cache.SetObjectWithJson(key, smsLimit, expire);
        }
    }

    internal class SmsLimitModel
    {
        public int Day { get; set; }

        public List<Record> Records { get; set; }

        public class Record
        {
            public int Hour { get; set; }

            public int Minute { get; set; }
        }
    }
}