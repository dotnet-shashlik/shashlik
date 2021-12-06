using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

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

        // 0:phone
        private const string DAY_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:DAY:{0}";
        // 0:phone
        private const string HOUR_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:HOURE:{0}";
        // 0:phone
        private const string MINUTE_CACHE_PREFIX = "SMS_MEMORYCAHCHE_LIMIT:MINUTE:{0}";

        public bool CanSend(string phone)
        {
            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone);
            if (Options.CurrentValue.CaptchaMinuteLimitCount > 0
                && Cache.TryGetValue<_MinuteCounter>(minuteCacheKey, out var minute)
                && minute.Counter >= Options.CurrentValue.CaptchaMinuteLimitCount)
                return false;

            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone);
            if (Options.CurrentValue.CaptchaHourLimitCount > 0
                && Cache.TryGetValue<_HourCounter>(hourCacheKey, out var hour)
                && hour.Counter >= Options.CurrentValue.CaptchaHourLimitCount)
                return false;

            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone);
            if (Options.CurrentValue.CaptchaDayLimitCount > 0
                && Cache.TryGetValue<_HourCounter>(dayCacheKey, out var day)
                && day.Counter >= Options.CurrentValue.CaptchaDayLimitCount)
                return false;

            return true;
        }

        public void SendDone(string phone)
        {
            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone);
            var dayCounter = Cache.GetOrCreate(dayCacheKey, r =>
            {
                r.SetSlidingExpiration(TimeSpan.FromDays(1));
                return new _DayCounter();
            });
            Interlocked.Increment(ref dayCounter.Counter);

            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone);
            var hourCounter = Cache.GetOrCreate(hourCacheKey, r =>
            {
                r.SetSlidingExpiration(TimeSpan.FromHours(1));
                return new _HourCounter();
            });
            Interlocked.Increment(ref hourCounter.Counter);

            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone);
            var minuteCounter = Cache.GetOrCreate(minuteCacheKey, r =>
            {
                r.SetSlidingExpiration(TimeSpan.FromMinutes(1));
                return new _MinuteCounter();
            });
            Interlocked.Increment(ref minuteCounter.Counter);
        }


        private class _DayCounter
        {
            public volatile int Counter;
        }

        private class _HourCounter
        {
            public volatile int Counter;
        }

        private class _MinuteCounter
        {
            public volatile int Counter;
        }
    }
}