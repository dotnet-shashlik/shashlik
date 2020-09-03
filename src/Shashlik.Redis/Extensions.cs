using System;
using System.Threading;
using CSRedis;

namespace Shashlik.Redis
{
    public static class Extensions
    {
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="key">锁key</param>
        /// <param name="lockSecond">锁定时长,秒</param>
        /// <param name="autoDelay">是否自动延期</param>
        /// <returns></returns>
        public static CSRedisLocker Locking(this CSRedisClient redisClient, string key,
            int lockSecond = 3, bool autoDelay = true)
        {
            key = $"CSRedisLocker:{key}";
            var value = Guid.NewGuid().ToString("n");

            while (true)
            {
                if (redisClient.Set(key, value, lockSecond, RedisExistence.Nx))
                    return new CSRedisLocker(redisClient, autoDelay, lockSecond, key, value);
                Thread.Sleep(10);
            }
        }
    }
}