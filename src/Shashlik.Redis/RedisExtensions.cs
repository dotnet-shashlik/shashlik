﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using FreeRedis;
using Shashlik.Utils.Extensions;

namespace Shashlik.Redis
{
    public static class RedisExtensions
    {
        private static ConstructorInfo CSRedisClientLockConstructorInfo { get; }

        static RedisExtensions()
        {
            CSRedisClientLockConstructorInfo =
                typeof(RedisClient.LockController).GetDeclaredConstructor(
                    typeof(RedisClient),
                    typeof(string),
                    typeof(string),
                    typeof(int),
                    typeof(double),
                    typeof(bool))!;

            if (CSRedisClientLockConstructorInfo is null)
                throw new MissingMemberException(nameof(RedisClient.LockController), "ctor");
        }

        /// <summary>
        /// 开启分布式锁，若超时抛出<see cref="RedisLockFailureException"/>异常
        /// </summary>
        /// <param name="redisClient"></param>
        /// <param name="name">锁名称</param>
        /// <param name="lockSeconds">锁定时长,区分官方的Lock方法</param>
        /// <param name="timeoutSeconds">等待锁超时（秒）</param>
        /// <param name="autoDelay">自动延长锁超时时间，看门狗线程的超时时间为lockSeconds/2 ， 在看门狗线程超时时间时自动延长锁的时间为lockSeconds。除非程序意外退出，否则永不超时。</param>
        /// <returns>锁对象,dispose即释放锁</returns>
        /// <exception cref="RedisLockFailureException"></exception>
        public static IDisposable Lock(
            this RedisClient redisClient,
            string name,
            int lockSeconds,
            int timeoutSeconds,
            bool autoDelay = true)
        {
            if (redisClient == null) throw new ArgumentNullException(nameof(redisClient));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            name = $"ShashlikCSRedisClientLock:{name}";

            var now = DateTime.Now;
            while (DateTime.Now.Subtract(now).TotalSeconds < timeoutSeconds)
            {
                var str = Guid.NewGuid().ToString();
                if (redisClient.SetNx(name, str, lockSeconds))
                {
                    var refreshSeconds = timeoutSeconds / 2.0;
                    return (IDisposable)CSRedisClientLockConstructorInfo.Invoke(
                        new object[] { redisClient, name, str, lockSeconds, refreshSeconds, autoDelay });
                }

                Task.Delay(3).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            throw new RedisLockFailureException(name);
        }
    }
}