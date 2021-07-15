using System;
using System.Linq;
using CSRedis;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Sms.Options;
using Shashlik.Utils.Extensions;

namespace Shashlik.Sms.Limit.Redis
{
    /// <summary>
    /// 分布式缓存短信发送限制
    /// </summary>
    [ConditionDependsOn(typeof(CSRedisClient))]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.Enable", true, DefaultValue = true)]
    [ConditionOnProperty(typeof(bool), "Shashlik.Sms.EnableRedisLimit", true, DefaultValue = true)]
    public class RedisSmsLimit : ISmsLimit
    {
        public RedisSmsLimit(IOptionsMonitor<SmsOptions> options, CSRedisClient redisClient)
        {
            Options = options;
            RedisClient = redisClient;
        }

        private IOptionsMonitor<SmsOptions> Options { get; }
        private CSRedisClient RedisClient { get; }

        // 0:phone,1:subject,2:hour,3:minute
        private const string CachePrefix = "SMS_REDIS_LIMIT:{0}:{1}:{2}:{3}";

        // 0:phone,1:subject
        private const string CachePrefixKeys = "SMS_REDIS_LIMIT:{0}:{1}";

        public bool CanSend(string phone, string subject)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));
            if (subject.Contains(':'))
                throw new ArgumentException($"Invalid subject of {subject}", nameof(subject));

            var limit = Options.CurrentValue.Limits.FirstOrDefault(r => r.Subject == subject);
            if (limit is null)
                return true;

            var keys = RedisClient.Keys($"{CachePrefixKeys.Format(phone, subject)}:*");
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
                if (data.Value.minute == minute && data.Value.hour == hour)
                {
                    minuteTotal += 1;
                    if (limit.MinuteLimitCount <= minuteTotal)
                        return false;
                }

                if (data.Value.hour == hour)
                {
                    hourTotal += 1;
                    if (limit.HourLimitCount <= hourTotal)
                        return false;
                }

                dayTotal += 1;
                if (limit.DayLimitCount <= dayTotal)
                    return false;
            }

            return true;
        }

        public void SendDone(string phone, string subject)
        {
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(phone));
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(subject));
            if (phone.Contains(':'))
                throw new ArgumentException($"Invalid phone of {phone}", nameof(phone));
            if (subject.Contains(':'))
                throw new ArgumentException($"Invalid subject of {subject}", nameof(subject));

            var limit = Options.CurrentValue.Limits.FirstOrDefault(r => r.Subject == subject);
            if (limit is null)
                return;
            var hour = DateTime.Now.Hour;
            var minute = DateTime.Now.Minute;

            var key = CachePrefix.Format(phone, subject, hour, minute);

            RedisClient.Set(key, string.Empty, (DateTime.Today.AddDays(1) - DateTime.Now));
        }

        private (int hour, int minute)? Convert(string key)
        {
            var arr = key.Split(new[] {':'});
            if (arr.Length != 5)
                return default;
            return (arr[3].ParseTo<int>(), arr[4].ParseTo<int>());
        }
    }
}