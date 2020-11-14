#nullable enable
using System;
using System.Threading;
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
            if (waitTimeoutSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(waitTimeoutSeconds));
            return Lock(key, lockSeconds, waitTimeoutSeconds, autoDelay) ?? throw new SynchronizationLockException();
        }

        // Lock copy from: https://github.com/2881099/csredis/blob/master/src/CSRedisCore/CSRedisClient.cs
        // copyright is CSRedisCore https://github.com/2881099/csredis/blob/master/LICENSE
        // add lockSeconds argument

        /// <summary>
        /// 开启分布式锁，若超时返回null
        /// copy from :
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="lockSeconds">锁定时长,区分官方的Lock方法</param>
        /// <param name="timeoutSeconds">超时（秒）</param>
        /// <param name="autoDelay">自动延长锁超时时间，看门狗线程的超时时间为timeoutSeconds/2 ， 在看门狗线程超时时间时自动延长锁的时间为timeoutSeconds。除非程序意外退出，否则永不超时。</param>
        /// <returns></returns>
        private CSRedisClientLock? Lock(string name, int lockSeconds, int timeoutSeconds, bool autoDelay = true)
        {
            name = $"CSRedisClientLock:{name}";
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalSeconds < timeoutSeconds)
            {
                var value = Guid.NewGuid().ToString();
                if (RedisClient.Set(name, value, lockSeconds, RedisExistence.Nx))
                {
                    return new CSRedisClientLock(RedisClient, name, value, lockSeconds, autoDelay);
                }

                Thread.CurrentThread.Join(3);
            }

            return null;
        }
    }
}