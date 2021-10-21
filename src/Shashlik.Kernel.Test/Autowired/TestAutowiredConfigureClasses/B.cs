using System;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    [BeforeAt(typeof(C)), Transient]
    public class B : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBe(nameof(A));
            CurrentValue.Value = nameof(B);
        }
    }
}