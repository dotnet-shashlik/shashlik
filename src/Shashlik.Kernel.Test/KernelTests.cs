using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Exceptions;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses.DependencyCondition;
using Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService1;
using Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService2;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Kernel.Test
{
    public class KernelTests : KernelTestBase
    {
        public KernelTests(TestWebApplicationFactory<TestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public void IntegrationTest()
        {
            {
                var memoryCache = GetService<IMemoryCache>();
                var op = memoryCache.Get<TestOptions3>("tt");
            }

            // AutoOptions
            {
                var options1 = GetService<IOptions<TestOptions1>>();
                options1.Value.Enable.ShouldBeTrue();

                var options2 = GetService<IOptions<TestOptions2>>();
                options2.Value.Enable.ShouldBeFalse();
            }

            // conditions
            {
                GetService<NeedTestOption1True>().ShouldNotBeNull();
                GetService<NeedTestOption2Miss>().ShouldNotBeNull();
                GetService<NeedTestOption4ZhangSan1>().ShouldBeNull();
                GetService<NeedTestOption4ZhangSan2>().ShouldBeNull();
                GetService<NeedTestOption4ZhangSan3>().ShouldNotBeNull();

                GetService<EnvConditionProd>().ShouldBeNull();
                GetService<EnvConditionDev>().ShouldNotBeNull();
                GetService<DependsOnNeedTestOption1True>().ShouldNotBeNull();
                GetService<DependsOnEnvConditionProd>().ShouldBeNull();
                GetService<DependsOnAny>().ShouldNotBeNull();
                GetService<DependsOnAnyShouldBeNull>().ShouldBeNull();
                GetService<DependsOnAll>().ShouldBeNull();
                GetService<DependsOnAllShouldBeNotNull>().ShouldNotBeNull();
                GetService<TestCondition>().ShouldBeNull();
                //GetService<BasedOn>().ShouldNotBeNull();
                GetService<FailConditionTestClass>().ShouldNotBeNull();
            }

            // autowired
            {
                GetService<IOptions<TestOptions3>>().Value.Name.ShouldBe("张三");
                AutowiredConfigure.Inited.ShouldBeTrue();
                AutowiredConfigureAspNetCore.Inited.ShouldBeTrue();
            }

            {
                (GetServices<IA1<int>>().Single() is D1<int>).ShouldBeTrue();
                GetService<B1<int>>().ShouldBeNull();
                GetService<C1<int>>().ShouldBeNull();
                (GetServices<D1<int?>>().Single() is D1<int?>).ShouldBeTrue();
            }

            {
                GetService<IA2<int>>().ShouldBeNull();
                (GetServices<B2<int>>().Single() is D2<int>).ShouldBeTrue();
                GetService<C2<int>>().ShouldBeNull();
                (GetServices<D2<int?>>().Single() is D2<int?>).ShouldBeTrue();
            }

            {
                (GetServices<IA3<int>>().Single() is C3<int>).ShouldBeTrue();
                (GetServices<B3<int>>().Single() is C3<int>).ShouldBeTrue();
                (GetServices<C3<int?>>().Single() is C3<int?>).ShouldBeTrue();
                GetService<D3<int>>().ShouldBeNull();
            }

            {
                (GetServices<IA4<int>>().Single() is D4<int>).ShouldBeTrue();
                (GetServices<B4<int>>().Single() is D4<int>).ShouldBeTrue();
                (GetServices<C4<int?>>().Single() is D4<int?>).ShouldBeTrue();
                (GetServices<D4<int?>>().Single() is D4<int?>).ShouldBeTrue();
            }

            {
                (GetServices<IComparer>().Any(r => r is C5<int>)).ShouldBeFalse();
                (GetServices<IDisposable>().Any(r => r is C5<int>)).ShouldBeFalse();

                GetServices<IA5<int>>().Any(r => r is C5<int>).ShouldBeFalse();
                GetServices<IA5<int>>().Any(r => r is D5<int>).ShouldBeFalse();

                GetServices<B5<int>>().Any(r => r is C5<int>).ShouldBeTrue();
                GetServices<B5<int>>().Any(r => r is D5<int>).ShouldBeTrue();

                GetServices<C5<int>>().Any(r => r is C5<int>).ShouldBeTrue();
                GetServices<C5<int>>().Any(r => r is D5<int>).ShouldBeTrue();

                GetServices<D5<int>>().Any(r => r is D5<int>).ShouldBeTrue();
            }

            {
                GetService<B16>().ShouldNotBeNull();
                GetService<C16>().ShouldNotBeNull();
                GetServices<IA16<int>>().Any(r => r is B16).ShouldBeTrue();
                GetServices<IA16<int>>().Any(r => r is C16).ShouldBeTrue();
                GetServices<IA16<string>>().Any(r => r is B16).ShouldBeTrue();
                GetServices<IA16<string>>().Any(r => r is C16).ShouldBeTrue();

                GetService<IA16<long>>().ShouldBeNull();
            }

            {
                GetService<IA17>().ShouldBeNull();
                GetService<B17<int>>().ShouldNotBeNull();
                GetService<B17<string>>().ShouldNotBeNull();
                GetService<C17<int>>().ShouldNotBeNull();
                GetService<C17<string>>().ShouldNotBeNull();
            }

            {
                GetService<IA18<string>>().ShouldNotBeNull();
                GetService<IA18<int>>().ShouldNotBeNull();
                GetService<IA18<long>>().ShouldBeNull();
                GetService<B18<int>>().ShouldNotBeNull();
                GetService<B18<string>>().ShouldBeNull();
                GetService<C18>().ShouldNotBeNull();
            }

            {
                (GetServices<IA1>().Single() is D1).ShouldBeTrue();
                GetService<B1>().ShouldBeNull();
                GetService<C1>().ShouldBeNull();
                (GetServices<D1>().Single() is D1).ShouldBeTrue();
            }

            {
                GetService<IA2>().ShouldBeNull();
                (GetServices<B2>().Single() is D2).ShouldBeTrue();
                GetService<C2>().ShouldBeNull();
                (GetServices<D2>().Single() is D2).ShouldBeTrue();
            }

            {
                (GetServices<IA3>().Single() is C3).ShouldBeTrue();
                (GetServices<B3>().Single() is C3).ShouldBeTrue();
                (GetServices<C3>().Single() is C3).ShouldBeTrue();
                GetService<D3>().ShouldBeNull();
            }

            {
                (GetServices<IA4>().Single() is D4).ShouldBeTrue();
                (GetServices<B4>().Single() is D4).ShouldBeTrue();
                (GetServices<C4>().Single() is D4).ShouldBeTrue();
                (GetServices<D4>().Single() is D4).ShouldBeTrue();
            }

            {
                (GetServices<IComparer>().Any(r => r is C5)).ShouldBeFalse();
                (GetServices<IDisposable>().Any(r => r is C5)).ShouldBeFalse();

                GetService<IA5>().ShouldBeNull();
                GetServices<B5>().Any(r => r is C5).ShouldBeTrue();
                GetServices<B5>().Any(r => r is D5).ShouldBeTrue();

                GetServices<C5>().Any(r => r is C5).ShouldBeTrue();
                GetServices<C5>().Any(r => r is D5).ShouldBeTrue();

                GetServices<D5>().Any(r => r is D5).ShouldBeTrue();
            }

            {
                (GetServices<IComparer>().Any(r => r is C6)).ShouldBeFalse();
                (GetServices<IDisposable>().Any(r => r is D6)).ShouldBeFalse();
                (GetServices<ICloneable>().Any(r => r is C6)).ShouldBeFalse();

                GetService<IA6>().ShouldBeNull();
                GetServices<IA61<int>>().Any(r => r is C6).ShouldBeTrue();
                GetServices<IA61<string>>().Any(r => r is C6).ShouldBeFalse();
                GetServices<IA61<int>>().Any(r => r is C61<int>).ShouldBeTrue();
                GetServices<IA61<string>>().Any(r => r is C61<string>).ShouldBeTrue();

                GetServices<B6>().Any(r => r is C6).ShouldBeTrue();
                GetServices<B6>().Any(r => r is C61<int>).ShouldBeFalse();

                GetServices<C6>().Any(r => r is C6).ShouldBeTrue();
                GetServices<C61<string>>().Any(r => r is C61<string>).ShouldBeTrue();

                GetServices<D6>().Any(r => r is D6).ShouldBeTrue();

                GetServices<IA61<int>>().Any(r => r is E6).ShouldBeFalse();
                GetServices<IA61<int>>().Any(r => r is F6).ShouldBeFalse();
            }

            {
                GetService<IA27>().ShouldBeNull();
                GetService<B27>().ShouldNotBeNull();
            }
        }

        /// <summary>
        /// MemoryLock测试
        /// </summary>
        [Fact]
        public void MemoryLockTest()
        {
            var locker = GetService<ILock>();

            {
                // 锁5秒，自动续期，10秒后释放，另一个锁等待11秒
                using var locker1 = locker.Lock("TestLock0", 5, true, 10);
                locker1.ShouldNotBeNull();
                TimerHelper.SetTimeout(locker1.Dispose, TimeSpan.FromSeconds(10));
                locker.Lock("TestLock0", 5, true, 11).ShouldNotBeNull();
            }

            {
                using var locker1 = locker.Lock("TestLock1", 30, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<LockFailureException>(() => locker.Lock("TestLock1", 5, true, 5));
            }

            {
                var locker1 = locker.Lock("TestLock2", 30, true, 5);
                locker1.ShouldNotBeNull();
                Should.Throw<LockFailureException>(() => locker.Lock("TestLock2", 5, true, 5));
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
                    Should.Throw<LockFailureException>(() => locker.Lock("TestLock3", 5, true, 5));
                }
            }

            {
                using var locker1 = locker.Lock("TestLock4", 30, true, 5);
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

                lockers.Count.ShouldBe(0);
            }

            {
                var lockers = new ConcurrentBag<IDisposable>();
                Parallel.For(1, 10, index =>
                {
                    try
                    {
                        var locker2 = locker.Lock("TestLock5", 30, true, 5);
                        lockers.Add(locker2);
                    }
                    catch
                    {
                        // ignored
                    }
                });

                lockers.Count.ShouldBe(1);
                lockers.First().Dispose();

                using var locker1 = locker.Lock("TestLock5", 5, true, 5);
                locker1.ShouldNotBeNull();
            }


            {
                using var locker1 = locker.Lock("TestLock6", 5, true, 1);
                locker1.ShouldNotBeNull();
                // 锁5秒，自动延期，10秒后仍然是锁定状态
                Thread.Sleep(10_000);
                Should.Throw<LockFailureException>(() => locker.Lock("TestLock6", 5, true, 5));
            }
        }
    }
}