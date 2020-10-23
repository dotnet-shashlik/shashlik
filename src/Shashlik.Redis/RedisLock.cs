using System;
using CSRedis;
using Shashlik.Kernel;
using Shashlik.Kernel.Locker;

namespace Shashlik.Redis
{
    public class RedisLock : ILock
    {
        public RedisLock(CSRedisClient redisClient)
        {
            RedisClient = redisClient;
        }

        private CSRedisClient RedisClient { get; }

        public IDisposable Lock(string key, int lockSecond, bool autoDelay = true, int waitTimeout = 60)
        {
            return RedisClient.Locking(key, lockSecond, autoDelay);
        }
    }
}