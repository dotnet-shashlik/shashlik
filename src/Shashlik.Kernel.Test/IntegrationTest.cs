using System;
using System.Collections;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses.DependencyCondition;
using Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService1;
using Shouldly;
using Xunit;

namespace Shashlik.Kernel.Test
{
    public class IntegrationTest : KernelTestBase
    {
        public IntegrationTest(TestWebApplicationFactory<TestStartup> factory) : base(factory)
        {
        }

        [Fact]
        public void DoTest()
        {
            // aop
            {
                var c = GetService<ICustomService>();
                c.Call1();
                c.Call2();
            }


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

                GetServices<IA5<int>>().Any(r => r is C5<int>).ShouldBeTrue();
                GetServices<IA5<int>>().Any(r => r is D5<int>).ShouldBeTrue();

                GetServices<B5<int>>().Any(r => r is C5<int>).ShouldBeTrue();
                GetServices<B5<int>>().Any(r => r is D5<int>).ShouldBeTrue();

                GetServices<C5<int>>().Any(r => r is C5<int>).ShouldBeTrue();
                GetServices<C5<int>>().Any(r => r is D5<int>).ShouldBeTrue();

                GetServices<D5<int>>().Any(r => r is D5<int>).ShouldBeTrue();
            }
        }

        [Fact]
        public void MemoryLockTest()
        {
            var locker = GetService<ILock>();
            var key = "lock_test";

            // 第一次直接锁没问题
            using var _ = locker.Lock(key, 3, false, 60);

            // 同步再来锁,异常
            Should.Throw<OperationCanceledException>(() => locker.Lock(key, 3, false, 1));

            // 3秒后可以锁
            using var @lock = locker.Lock(key, 1, false, 3);
        }
    }
}