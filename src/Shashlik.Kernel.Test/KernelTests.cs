using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Test.Autowired;
using Shashlik.Kernel.Test.Autowired.TestAutowireConditionClasses;
using Shashlik.Kernel.Test.Options;
using Shashlik.Kernel.Test.TestClasses.DependencyCondition;
using Shashlik.Kernel.Test.TestClasses.ServiceTests;
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
                GetService<FailConditionTestClass>().ShouldNotBeNull();
            }

            // autowired
            {
                GetService<IOptions<TestOptions3>>().Value.Name.ShouldBe("张三");
                AutowiredConfigure.Inited.ShouldBeTrue();
            }

            {
                GetService<IA>().ShouldBeOfType<A2>();
                var enumerable = GetServices<IA>();
                enumerable.Count().ShouldBe(3);
                GetServices<A1>().ShouldNotBeNull();
                GetServices<A2>().ShouldNotBeNull();
                GetServices<A3>().ShouldNotBeNull();

                GetService<IB<int>>().ShouldBeOfType<B1>();
                GetService<IB<string>>().ShouldBeOfType<B2<String>>();
                GetService<B1>().ShouldNotBeNull();
                GetService<B2<int>>().ShouldNotBeNull();
                GetService<B2<String>>().ShouldNotBeNull();

                GetService<IC>().ShouldBeNull();
                GetService<C<int>>().ShouldNotBeNull();
                GetService<C<String>>().ShouldNotBeNull();
            }
        }
    }
}