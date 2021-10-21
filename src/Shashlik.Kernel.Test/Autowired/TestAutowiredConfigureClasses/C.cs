using System;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    [BeforeAt(typeof(D)),Transient]
    public class C : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBe(nameof(B));
            CurrentValue.Value = nameof(C);
        }
    }
}