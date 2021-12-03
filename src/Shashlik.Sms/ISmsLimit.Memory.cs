using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Sms
{
    /// <summary>
    /// 基于内存缓存的短信发送限制
    /// </summary>
    [Singleton]
    [ConditionDependsOn(typeof(IMemoryCache))]
    [ConditionDependsOnMissing(typeof(ISmsLimit))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    public class MemorySmsLimit : ISmsLimit, IDisposable
    {
        public MemorySmsLimit(IMemoryCache cache, IOptionsMonitor<SmsOptions> options)
        {
            Cache = cache;
            Options = options;
        }

        private IMemoryCache Cache { get; }
        private IOptionsMonitor<SmsOptions> Options { get; }

        private readonly ConcurrentDictionary<string, AsyncLock> _asyncLocks = new ConcurrentDictionary<string, AsyncLock>();

        // 0:phone,1:subject
        private const string CachePrefix = "SMS_MEMORYCAHCHE_LIMIT:{0}";

        public bool CanSend(string phone)
        {
            string key = CachePrefix.Format(phone);
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var smsLimit = Cache.Get<SmsLimitModel>(key);
            if (smsLimit is null)
                return true;
            if (Options.CurrentValue.CaptchaDayLimitCount > 0
                && Options.CurrentValue.CaptchaDayLimitCount <= smsLimit.DayCounter
            )
                return false;
            if (Options.CurrentValue.CaptchaHourLimitCount > 0
                && Options.CurrentValue.CaptchaHourLimitCount <= smsLimit.HourCounter
                && smsLimit.Hour == hour
            )
                return false;
            if (Options.CurrentValue.CaptchaMinuteLimitCount > 0
                && Options.CurrentValue.CaptchaMinuteLimitCount <= smsLimit.HourCounter
                && smsLimit.Minute == minute)
                return false;

            return true;
        }

        public void SendDone(string phone)
        {
            //TODO: 优化内存使用和算法

            var key = CachePrefix.Format(phone);
            using var @lock = _asyncLocks.GetOrAdd(key, new AsyncLock());
            var day = DateTime.Now.Day;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            var smsLimit = Cache.Get<SmsLimitModel>(key) ?? new SmsLimitModel();
            smsLimit.DayCounter++;

            if (smsLimit.Hour == hour)
                smsLimit.HourCounter++;
            else
            {
                smsLimit.Hour = hour;
                smsLimit.HourCounter = 1;
            }

            if (smsLimit.Minute == minute)
                smsLimit.MinuteCounter++;
            else
            {
                smsLimit.Minute = minute;
                smsLimit.MinuteCounter = 1;
            }

            Cache.Set(key, smsLimit, DateTimeOffset.Now.Date.AddDays(1));
        }

        public void Dispose()
        {
            _asyncLocks.Values.ForEachItem(x => x.Dispose());
        }
    }

    internal class SmsLimitModel
    {
        public int DayCounter { get; set; }
        public int Hour { get; set; }
        public int HourCounter { get; set; }
        public int Minute { get; set; }
        public int MinuteCounter { get; set; }
    }
}