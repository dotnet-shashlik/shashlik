using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses.DependencyCondition;
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
                GetService<BasedOn>().ShouldNotBeNull();
            }

            // autowired
            {
                GetService<IOptions<TestOptions3>>().Value.Name.ShouldBe("张三");
                AutowiredConfigure.Inited.ShouldBeTrue();
                AutowiredConfigureAspNetCore.Inited.ShouldBeTrue();
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