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

        // 0:phone,1:hour,2:minute
        private const string CachePrefix = "SMS_REDIS_LIMIT:{0}:{1}:{2}";

        // 0:phone
        private const string CachePrefixKeys = "SMS_REDIS_LIMIT:{0}";

        public bool CanSend(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));

            var keys = RedisClient.Keys($"{CachePrefixKeys.Format(phone)}:*");
            if (keys.IsNullOrEmpty())
                return true;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;
            int minuteTotal = 0, hourTotal = 0, dayTotal = 0;
            foreach (var item in keys)
            {
                var data = Convert(item);
                if (data is null)
                    continue;
                if (data.Value.hour == hour && data.Value.minute == minute)
                {
                    minuteTotal += 1;
                    if (Options.CurrentValue.CaptchaMinuteLimitCount > 0 && Options.CurrentValue.CaptchaMinuteLimitCount <= minuteTotal)
                        return false;
                }

                if (data.Value.hour == hour)
                {
                    hourTotal += 1;
                    if (Options.CurrentValue.CaptchaHourLimitCount > 0 && Options.CurrentValue.CaptchaHourLimitCount <= hourTotal)
                        return false;
                }

                dayTotal += 1;
                if (Options.CurrentValue.CaptchaDayLimitCount > 0 && Options.CurrentValue.CaptchaDayLimitCount <= dayTotal)
                    return false;
            }

            return true;
        }

        public void SendDone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));

            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var key = CachePrefix.Format(phone, hour, minute);

            RedisClient.Set(key, string.Empty, (DateTime.Today.AddDays(1) - DateTime.Now));
        }

        private (int hour, int minute)? Convert(string key)
        {
            var arr = key.Split(new[] { ':' });
            if (arr.Length != 4)
                return default;
            return (arr[2].ParseTo<int>(), arr[3].ParseTo<int>());
        }
    }
}