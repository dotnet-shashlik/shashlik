using System;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Autowired.Attributes;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    [BeforeAt(typeof(D))]
    public class C : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBe(nameof(B));
            CurrentValue.Value = nameof(B);
        }
    }
}