using System;
using CSRedis;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Exceptions;

namespace Shashlik.Redis
{
    [ConditionOnProperty(typeof(bool), "Shashlik.Redis.Enable", true, DefaultValue = true)]
    [Singleton]
    public class RedisLock
    {
        public RedisLock(CSRedisClient redisClient)
        {
            RedisClient = redisClient;
        }

        private CSRedisClient RedisClient { get; }

        /// <summary>
        /// lockSecond参数无效
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lockSeconds"></param>
        /// <param name="autoDelay"></param>
        /// <param name="waitTimeoutSeconds"></param>
        /// <returns></returns>
        public IDisposable Lock(string key, int lockSeconds, bool autoDelay = true, int waitTimeoutSeconds = 60)
        {
            if (lockSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(lockSeconds));
            if (waitTimeoutSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(waitTimeoutSeconds));

            return RedisClient.Lock(key, lockSeconds, waitTimeoutSeconds, autoDelay) ??
                   throw new LockFailureException(key);
        }
    }
}