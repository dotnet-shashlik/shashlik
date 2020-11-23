using System;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses.DependencyCondition;
using Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService1;
using Shashlik.Kernel.Test.TestClasses.ServiceTests.TestService2;
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