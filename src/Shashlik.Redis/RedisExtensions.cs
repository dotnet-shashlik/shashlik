using System;
using System.Threading;
using CSRedis;

namespace Shashlik.Redis
{
    public static class RedisExtensions
    {
        /// <summary>
        /// 分布式锁
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="key">锁key</param>
        /// <param name="lockSecond">锁定时长,秒</param>
        /// <param name="autoDelay">是否自动延期</param>
        /// <param name="waitTimeout">等待锁的超时时长</param>
        /// <returns></returns>
        public static CSRedisLocker Locking(this CSRedisClient redisClient, string key,
            int lockSecond = 3, bool autoDelay = true, int waitTimeout = 60)
        {
            key = $"CSRedisLocker:{key}";
            var value = Guid.NewGuid().ToString("n");


            var start = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - start).TotalSeconds >= waitTimeout)
                    throw new OperationCanceledException($"cannot get redis lock: {key}");

                if (redisClient.Set(key, value, lockSecond, RedisExistence.Nx))
                    return new CSRedisLocker(redisClient, autoDelay, lockSecond, key, value);
                Thread.Sleep(10);
            }
        }
    }
}