using System;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    [AfterAt(typeof(C)), Transient]
    public class D : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBe(nameof(C));
            CurrentValue.Value = nameof(D);
        }
    }
}