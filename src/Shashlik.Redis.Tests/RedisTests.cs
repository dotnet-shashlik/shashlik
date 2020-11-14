using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSRedis;
using Shashlik.Kernel;
using Shashlik.Kernel.Test;
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
                using var locker1 = RedisHelper.Lock("TestLock1", 5);
                locker1.ShouldNotBeNull();
                using var locker2 = RedisHelper.Lock("TestLock1", 5);
                locker2.ShouldBeNull();
            }

            {
                var locker1 = RedisHelper.Lock("TestLock2", 5, false);
                locker1.ShouldNotBeNull();
                var locker2 = RedisHelper.Lock("TestLock2", 5);
                locker2.ShouldBeNull();
                locker1.Dispose();
                using var locker3 = RedisHelper.Lock("TestLock2", 5);
                locker3.ShouldNotBeNull();
            }

            {
                using var locker1 = RedisHelper.Lock("TestLock3", 5);
                locker1.ShouldNotBeNull();

                var count = 5;
                for (var i = 0; i < count; i++)
                {
                    Thread.Sleep(5000);
                    using var locker2 = RedisHelper.Lock("TestLock3", 5);
                    locker2.ShouldBeNull();
                }
            }

            {
                using var locker1 = RedisHelper.Lock("TestLock4", 5);
                locker1.ShouldNotBeNull();

                var lockers = new ConcurrentBag<CSRedis.CSRedisClientLock>();
                Parallel.For(1, 10, index =>
                {
                    using var locker2 = RedisHelper.Lock("TestLock4", 5);
                    lockers.Add(locker2);
                });

                lockers.Count(r => r != null).ShouldBe(0);
            }

            {
                var lockers = new ConcurrentBag<CSRedis.CSRedisClientLock>();
                Parallel.For(1, 10, index =>
                {
                    var locker2 = RedisHelper.Lock("TestLock5", 5);
                    lockers.Add(locker2);
                });

                lockers.Count(r => r != null).ShouldBe(1);

                foreach (var csRedisClientLock in lockers)
                {
                    csRedisClientLock?.Dispose();
                }

                using var locker1 = RedisHelper.Lock("TestLock5", 5);
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

            {
                using var locker1 = locker.Lock("TestLock1", 5, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<Exception>(() => locker.Lock("TestLock1", 5, true, 5));
            }

            {
                var locker1 = locker.Lock("TestLock2", 5, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<Exception>(() => locker.Lock("TestLock2", 5, true, 5));
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
                    Should.Throw<Exception>(() => locker.Lock("TestLock3", 5, true, 5));
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
        }

        [Fact]
        public void RedisSnowflakeIdTest()
        {
            RedisSnowflakeId.DatacenterId.ShouldNotBeNull();
            RedisSnowflakeId.WorkerId.ShouldNotBeNull();
            RedisSnowflakeId.IdWorker.NextId().ToString().Length.ShouldBe(19);
        }
    }
}