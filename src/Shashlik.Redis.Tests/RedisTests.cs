using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Shashlik.Kernel;
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
        /// CSRedisCore原生锁测试
        /// </summary>
        [Fact]
        public void OriginalLockTest()
        {
            {
                using var locker1 = RedisHelper.Lock("OriginTestLock1", 5);
                locker1.ShouldNotBeNull();
                using var locker2 = RedisHelper.Lock("OriginTestLock1", 5);
                locker2.ShouldBeNull();
            }

            {
                var locker1 = RedisHelper.Lock("OriginTestLock2", 5, false);
                locker1.ShouldNotBeNull();
                var locker2 = RedisHelper.Lock("OriginTestLock2", 5);
                locker2.ShouldBeNull();
                locker1.Dispose();
                using var locker3 = RedisHelper.Lock("OriginTestLock2", 5);
                locker3.ShouldNotBeNull();
            }

            {
                using var locker1 = RedisHelper.Lock("OriginTestLock3", 5);
                locker1.ShouldNotBeNull();

                var count = 5;
                for (var i = 0; i < count; i++)
                {
                    Thread.Sleep(5000);
                    using var locker2 = RedisHelper.Lock("OriginTestLock3", 5);
                    locker2.ShouldBeNull();
                }
            }

            {
                using var locker1 = RedisHelper.Lock("OriginTestLock4", 5);
                locker1.ShouldNotBeNull();

                var lockers = new ConcurrentBag<CSRedis.CSRedisClientLock>();
                Parallel.For(1, 10, index =>
                {
                    using var locker2 = RedisHelper.Lock("OriginTestLock4", 5);
                    lockers.Add(locker2);
                });

                lockers.Count(r => r != null).ShouldBe(0);
            }

            {
                var lockers = new ConcurrentBag<CSRedis.CSRedisClientLock>();
                Parallel.For(1, 10, index =>
                {
                    var locker2 = RedisHelper.Lock("OriginTestLock5", 5);
                    lockers.Add(locker2);
                });

                lockers.Count(r => r != null).ShouldBe(1);

                foreach (var csRedisClientLock in lockers)
                {
                    csRedisClientLock?.Dispose();
                }

                using var locker1 = RedisHelper.Lock("OriginTestLock5", 5);
                locker1.ShouldNotBeNull();
            }
        }

        /// <summary>
        /// RedisLock测试
        /// </summary>
        [Fact]
        public void RedisLockTest()
        {
            var locker = GetService<ILock>();
            locker.ShouldBeOfType<RedisLock>();

            {
                using var locker1 = locker.Lock("TestLock0", 5, true, 10);
                locker1.ShouldNotBeNull();
                TimerHelper.SetTimeout(locker1.Dispose, TimeSpan.FromSeconds(10));
                locker.Lock("TestLock0", 5, true, 11).ShouldNotBeNull();
            }

            {
                using var locker1 = locker.Lock("TestLock1", 5, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<InvalidOperationException>(() => locker.Lock("TestLock1", 5, true, 5));
            }

            {
                var locker1 = locker.Lock("TestLock2", 5, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<InvalidOperationException>(() => locker.Lock("TestLock2", 5, true, 5));
                locker1.Dispose();
                using var locker3 = locker.Lock("TestLock2", 5, true, 5);
                locker3.ShouldNotBeNull();
            }

            {
                using var locker1 = locker.Lock("TestLock3", 5, true, 5);
                locker1.ShouldNotBeNull();

                var count = 5;
                for (var i = 0; i < count; i++)
                {
                    Thread.Sleep(5000);
                    Should.Throw<InvalidOperationException>(() => locker.Lock("TestLock3", 5, true, 5));
                }
            }

            {
                using var locker1 = locker.Lock("TestLock4", 5, true, 5);
                locker1.ShouldNotBeNull();

                var lockers = new ConcurrentBag<IDisposable>();
                Parallel.For(1, 10, index =>
                {
                    try
                    {
                        using var locker2 = locker.Lock("TestLock4", 5, true, 5);
                        lockers.Add(locker2);
                    }
                    catch
                    {
                        // ignored
                    }
                });

                lockers.Count(r => r != null).ShouldBe(0);
            }

            {
                var lockers = new ConcurrentBag<IDisposable>();
                Parallel.For(1, 10, index =>
                {
                    try
                    {
                        var locker2 = locker.Lock("TestLock5", 5, true, 5);
                        lockers.Add(locker2);
                    }
                    catch
                    {
                        // ignored
                    }
                });

                lockers.Count(r => r != null).ShouldBe(1);

                foreach (var csRedisClientLock in lockers)
                {
                    csRedisClientLock?.Dispose();
                }

                using var locker1 = locker.Lock("TestLock5", 5, true, 5);
                locker1.ShouldNotBeNull();
            }


            {
                using var locker1 = locker.Lock("TestLock6", 5, true, 1);
                locker1.ShouldNotBeNull();
                // 锁5秒，自动延期，10秒后仍然是锁定状态
                Thread.Sleep(10_000);
                Should.Throw<InvalidOperationException>(() => locker.Lock("TestLock6", 5, true, 5));
            }
        }

        [Fact]
        public void RedisSnowflakeIdTest()
        {
            RedisSnowflakeId.DatacenterId.ShouldNotBeNull();
            RedisSnowflakeId.WorkerId.ShouldNotBeNull();
            RedisSnowflakeId.IdWorker.NextId().ToString().ShouldNotBeNullOrWhiteSpace();

            var snowflakeId = GetService<IRedisSnowflakeId>();
            HashSet<int> ids = new HashSet<int>();
            HashSet<string> ids1 = new HashSet<string>();
            for (int i = 0; i < 2048; i++)
            {
                var (workId, dcId) = snowflakeId.GetId();
                (workId >= 0 && workId <= SnowflakeId.MaxWorkerId).ShouldBeTrue();
                (dcId >= 0 && dcId <= SnowflakeId.MaxDatacenterId).ShouldBeTrue();

                var redisId = (workId << 5) + dcId;
                (redisId >= 0 && dcId <= SnowflakeId.MaxDatacenterId * SnowflakeId.MaxWorkerId).ShouldBeTrue();
                ids.Add(redisId);
                ids1.Add($"{workId:D2}:{dcId:D2}");
            }

            ids.Count.ShouldBe(1024);
            ids1.Count.ShouldBe(1024);
        }

        [Fact]
        public void DistributedCacheTest()
        {
            var cache = GetService<IDistributedCache>();

            {
                cache.SetObjectWithJson("unit_redis_test", null, DateTimeOffset.Now.AddSeconds(5));
                var cacheObj = cache.GetObjectWithJson<int?>("unit_redis_test");
                cacheObj.ShouldBe(null);
            }

            {
                var cacheObj = cache.GetObjectWithJson<int?>("absolute_not_exists");
                cacheObj.ShouldBe(null);
            }

            {
                cache.SetObjectWithJson("unit_redis_test", 1, DateTimeOffset.Now.AddSeconds(5));
                var cacheObj = cache.GetObjectWithJson<int?>("unit_redis_test");
                cacheObj.ShouldBe(1);
            }
        }
    }
}