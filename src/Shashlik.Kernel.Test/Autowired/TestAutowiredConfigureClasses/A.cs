using System;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    // 无顺序要求
    [Transient]
    public class A : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBeNull();
            CurrentValue.Value = nameof(A);
        }
    }
}