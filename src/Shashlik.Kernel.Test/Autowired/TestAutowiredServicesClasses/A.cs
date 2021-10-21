using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesClasses
{
    // 无顺序要求
    [Transient]
    public class A : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBeNull();
            CurrentValue.Value = nameof(A);
        }
    }
}