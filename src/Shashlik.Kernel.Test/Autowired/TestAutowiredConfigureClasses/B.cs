using System;
using Shashlik.Kernel.Autowire.Attributes;
using  Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    [BeforeAt(typeof(C))]
    public class B : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBe(nameof(A));
            CurrentValue.Value = nameof(B);
        }
    }
}