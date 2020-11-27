using System;
using CSRedis;
using Shashlik.Kernel;

namespace Shashlik.Redis
{
    public class RedisLock : ILock
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
                   throw new InvalidOperationException($"Can't get redis lock: {key}");
        }
    }
}