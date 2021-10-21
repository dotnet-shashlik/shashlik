using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesOrderClasses
{
    // 无顺序要求
    [Order(4),Transient]
    public class A : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(D));
            CurrentValue.Value = nameof(A);
        }
    }
}