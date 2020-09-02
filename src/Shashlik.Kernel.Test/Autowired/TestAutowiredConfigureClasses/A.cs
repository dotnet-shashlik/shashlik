using System;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredConfigureClasses
{
    // 无顺序要求
    public class A : ITestAutowiredConfigure
    {
        public void Configure(IServiceProvider serviceProvider)
        {
            CurrentValue.Value.ShouldBeNull();
            CurrentValue.Value = nameof(A);
        }
    }
}