using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Shashlik.Kernel.Test;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Redis.Tests
{
    public class RedisTests : KernelTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RedisTests(TestWebApplicationFactory<TestStartup> factory, ITestOutputHelper testOutputHelper) :
            base(factory)
        {
            _testOutputHelper = testOutputHelper;
        }
    
        /// <summary>
        /// RedisLock测试
        /// </summary>
        [Fact]
        public void RedisLockTest()
        {
            var redisClient = GetService<RedisClient>();

            {
                // 锁5秒，自动续期，10秒后释放，另一个锁等待11秒
                using var locker1 = redisClient.Lock("TestLock0", 5, true, 10);
                locker1.ShouldNotBeNull();
                TimerHelper.SetTimeout(locker1.Dispose, TimeSpan.FromSeconds(10));
                redisClient.Lock("TestLock0", 5, true, 11).ShouldNotBeNull();
            }

            {
                using var locker1 = redisClient.Lock("TestLock1", 30, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<RedisLockFailureException>(() => redisClient.Lock("TestLock1", 5, true, 5));
            }

            {
                var locker1 = redisClient.Lock("TestLock2", 30, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<RedisLockFailureException>(() => redisClient.Lock("TestLock2", 5, true, 5));
                locker1.Dispose();
                using var locker3 = redisClient.Lock("TestLock2", 5, true, 5);
                locker3.ShouldNotBeNull();
            }

            {
                using var locker1 = redisClient.Lock("TestLock3", 5, true, 5);
                locker1.ShouldNotBeNull();

                var count = 5;
                for (var i = 0; i < count; i++)
                {
                    Thread.Sleep(5000);
                    Should.Throw<RedisLockFailureException>(() => redisClient.Lock("TestLock3", 5, true, 5));
                }
            }

            {
                using var locker1 = redisClient.Lock("TestLock4", 30, true, 5);
                locker1.ShouldNotBeNull();

                var lockers = new ConcurrentBag<IDisposable>();
                Parallel.For(1, 10, new ParallelOptions { MaxDegreeOfParallelism = 3 }, index =>
                {
                    try
                    {
                        using var locker2 = redisClient.Lock("TestLock4", 5, true, 5);
                        lockers.Add(locker2);
                    }
                    catch
                    {
                        // ignored
                    }
                });

                lockers.Count.ShouldBe(0);
            }

            {
                var lockers = new ConcurrentBag<IDisposable>();
                Parallel.For(1, 10, new ParallelOptions { MaxDegreeOfParallelism = 3 }, index =>
                {
                    try
                    {
                        var locker2 = redisClient.Lock("TestLock5", 30, true, 5);
                        lockers.Add(locker2);
                    }
                    catch
                    {
                        // ignored
                    }
                });

                lockers.Count.ShouldBe(1);
                lockers.First().Dispose();

                using var locker1 = redisClient.Lock("TestLock5", 5, true, 5);
                locker1.ShouldNotBeNull();
            }


            {
                using var locker1 = redisClient.Lock("TestLock6", 5, true, 1);
                locker1.ShouldNotBeNull();
                // 锁5秒，自动延期，10秒后仍然是锁定状态
                Thread.Sleep(10_000);
                Should.Throw<RedisLockFailureException>(() => redisClient.Lock("TestLock6", 5, true, 5));
            }
        }

        // [Fact]
        // public void DistributedCacheTest()
        // {
        //     var cache = GetService<IDistributedCache>();
        //
        //     {
        //         cache.SetObjectWithJson("unit_redis_test", null, DateTimeOffset.Now.AddSeconds(5));
        //         var cacheObj = cache.GetObjectWithJson<int?>("unit_redis_test");
        //         cacheObj.ShouldBe(null);
        //     }
        //
        //     {
        //         var cacheObj = cache.GetObjectWithJson<int?>("absolute_not_exists");
        //         cacheObj.ShouldBe(null);
        //     }
        //
        //     {
        //         cache.SetObjectWithJson("unit_redis_test", 1, DateTimeOffset.Now.AddSeconds(5));
        //         var cacheObj = cache.GetObjectWithJson<int?>("unit_redis_test");
        //         cacheObj.ShouldBe(1);
        //     }
        // }
    }

    internal static class LockExtension
    {
        public static IDisposable Lock(this RedisClient redisClient, string name, int lockSecond, bool autoDelay, int waitSecond)
        {
            return redisClient.Lock(name, lockSecond, waitSecond, autoDelay);
        }
    }
}