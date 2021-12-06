using System;
using CSRedis;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Limit.Redis
{
    /// <summary>
    /// 基于redis的短信发送频率限制
    /// </summary>
    [Singleton]
    [ConditionDependsOn(typeof(CSRedisClient))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(SmsOptions.Enable), true, DefaultValue = true)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms." + nameof(RedisSmsLimitOptions.EnableRedisLimit), true, DefaultValue = true)]
    public class RedisSmsLimit : ISmsLimit
    {
        public RedisSmsLimit(IOptionsMonitor<SmsOptions> options, CSRedisClient redisClient)
        {
            Options = options;
            RedisClient = redisClient;
        }

        private IOptionsMonitor<SmsOptions> Options { get; }

        private CSRedisClient RedisClient { get; }

        // 0:phone
        private const string DAY_CACHE_PREFIX = "SMS_REDIS_LIMIT:DAY:{0}";
        // 0:phone
        private const string HOUR_CACHE_PREFIX = "SMS_REDIS_LIMIT:HOURE:{0}";
        // 0:phone
        private const string MINUTE_CACHE_PREFIX = "SMS_REDIS_LIMIT:MINUTE:{0}";

        public bool CanSend(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));

            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone);
            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone);
            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone);
            if (Options.CurrentValue.CaptchaDayLimitCount > 0)
            {
                var counter = RedisClient.Get<int?>(dayCacheKey);
                if (counter.HasValue && Options.CurrentValue.CaptchaDayLimitCount <= counter.Value)
                    return false;
            }

            if (Options.CurrentValue.CaptchaHourLimitCount > 0)
            {
                var counter = RedisClient.Get<int?>(hourCacheKey);
                if (counter.HasValue && Options.CurrentValue.CaptchaHourLimitCount <= counter.Value)
                    return false;
            }

            if (Options.CurrentValue.CaptchaMinuteLimitCount > 0)
            {
                var counter = RedisClient.Get<int?>(minuteCacheKey);
                if (counter.HasValue && Options.CurrentValue.CaptchaMinuteLimitCount <= counter.Value)
                    return false;
            }

            return true;
        }

        public void SendDone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));

            var now = DateTime.Now;
            var daySeconds = (int)((now.Date.AddDays(1) - now).TotalSeconds);
            var hourSeconds = (int)((new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1) - now).TotalSeconds);
            var minuteSeconds = 60 - now.Second;

            var dayCacheKey = DAY_CACHE_PREFIX.Format(phone);
            var hourCacheKey = HOUR_CACHE_PREFIX.Format(phone);
            var minuteCacheKey = MINUTE_CACHE_PREFIX.Format(phone);

            string script = $@"
local dayKey = '{dayCacheKey}'
local dayCounter = redis.call('INCR','{dayCacheKey}')
if(dayCounter == 1)
then
  redis.call('EXPIRE','{dayCacheKey}',{daySeconds})
end

local hourKey = '{hourCacheKey}'
local hourCounter = redis.call('INCR','{hourCacheKey}')
if(hourCounter == 1)
then
    redis.call('EXPIRE','{hourCacheKey}',{hourSeconds})
end

local minuteKey = '{minuteCacheKey}'
local minuteCounter = redis.call('INCR','{minuteCacheKey}')
if(minuteCounter == 1)
then
    redis.call('EXPIRE','{minuteCacheKey}',{minuteSeconds})
end
";
            RedisClient.Eval(script, "NONE");
        }
    }
}