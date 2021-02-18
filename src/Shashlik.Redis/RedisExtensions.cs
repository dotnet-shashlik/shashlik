using System;
using System.Reflection;
using System.Threading.Tasks;
using CSRedis;
using Shashlik.Utils.Extensions;

namespace Shashlik.Redis
{
    public static class RedisExtensions
    {
        private static ConstructorInfo? CSRedisClientLockConstructorInfo { get; }

        static RedisExtensions()
        {
            CSRedisClientLockConstructorInfo =
                typeof(CSRedisClientLock).GetDeclaredConstructor(
                    typeof(CSRedisClient),
                    typeof(string),
                    typeof(string),
                    typeof(int),
                    typeof(double),
                    typeof(bool));

            if (CSRedisClientLockConstructorInfo is null)
                throw new MissingMemberException(nameof(CSRedisClientLock), "ctor");
        }

        // Lock copy from: https://github.com/2881099/csredis/blob/master/src/CSRedisCore/CSRedisClient.cs
        // copyright is CSRedisCore https://github.com/2881099/csredis/blob/master/LICENSE
        // add lockSeconds argument

        /// <summary>
        /// 开启分布式锁，若超时返回null
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="name">锁名称</param>
        /// <param name="lockSeconds">锁定时长,区分官方的Lock方法</param>
        /// <param name="timeoutSeconds">等待锁超时（秒）</param>
        /// <param name="autoDelay">自动延长锁超时时间，看门狗线程的超时时间为lockSeconds/2 ， 在看门狗线程超时时间时自动延长锁的时间为lockSeconds。除非程序意外退出，否则永不超时。</param>
        /// <returns></returns>
        public static CSRedisClientLock? Lock(
            this CSRedisClient redisClient,
            string name,
            int lockSeconds,
            int timeoutSeconds,
            bool autoDelay = true)
        {
            if (redisClient == null) throw new ArgumentNullException(nameof(redisClient));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            name = $"ShashlikCSRedisClientLock:{name}";
            var startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalSeconds < timeoutSeconds)
            {
                var value = Guid.NewGuid().ToString();
                if (redisClient.Set(name, value, lockSeconds, RedisExistence.Nx))
                {
                    var refreshSeconds = lockSeconds / 2.0;
                    return (CSRedisClientLock) CSRedisClientLockConstructorInfo!.Invoke(
                        new object[] {redisClient, name, value, lockSeconds, refreshSeconds, autoDelay});
                }

                Task.Delay(3).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            return null;
        }
    }
}