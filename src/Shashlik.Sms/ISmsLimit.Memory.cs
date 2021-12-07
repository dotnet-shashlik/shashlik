using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

// ReSharper disable InconsistentNaming

namespace Shashlik.Sms
{
    /// <summary>
    /// 基于内存缓存的短信发送限制
    /// </summary>
    [Singleton]
    [ConditionDependsOn(typeof(IMemoryCache))]
    [ConditionDependsOnMissing(typeof(ISmsLimit))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
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
        private const string DAY_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:DAY:{0}:{1}";

        // 0:phone,1:subject
        private const string HOUR_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:HOURE:{0}:{1}";

        // 0:phone,1:subject
        private const string MINUTE_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:MINUTE:{0}:{1}";

        public bool CanSend(string phone, string subject)
        {
            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone, subject);
            if (Options.CurrentValue.CaptchaMinuteLimitCount > 0
                && Cache.TryGetValue<Counter>(minuteCacheKey, out var minute)
                && minute._counter >= Options.CurrentValue.CaptchaMinuteLimitCount)
                return false;

            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone, subject);
            if (Options.CurrentValue.CaptchaHourLimitCount > 0
                && Cache.TryGetValue<Counter>(hourCacheKey, out var hour)
                && hour._counter >= Options.CurrentValue.CaptchaHourLimitCount)
                return false;

            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone, subject);
            if (Options.CurrentValue.CaptchaDayLimitCount > 0
                && Cache.TryGetValue<Counter>(dayCacheKey, out var day)
                && day._counter >= Options.CurrentValue.CaptchaDayLimitCount)
                return false;

            return true;
        }

        public void SendDone(string phone, string subject)
        {
            var now = DateTimeOffset.Now;

            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone, subject);
            var dayCounter = Cache.GetOrCreate(dayCacheKey, r =>
            {
                r.SetAbsoluteExpiration(now.Date.AddDays(1));
                return new Counter();
            });
            Interlocked.Increment(ref dayCounter._counter);

            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone, subject);
            var hourCounter = Cache.GetOrCreate(hourCacheKey, r =>
            {
                r.SetAbsoluteExpiration(
                    new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeZoneInfo.Local.BaseUtcOffset).AddHours(1)
                );
                return new Counter();
            });
            Interlocked.Increment(ref hourCounter._counter);

            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone, subject);
            var minuteCounter = Cache.GetOrCreate(minuteCacheKey, r =>
            {
                r.SetAbsoluteExpiration(
                    new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, TimeZoneInfo.Local.BaseUtcOffset).AddMinutes(1)
                );
                return new Counter();
            });
            Interlocked.Increment(ref minuteCounter._counter);
        }

        private class Counter
        {
            public volatile int _counter;
        }
    }
}