using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesClasses
{
    // 无顺序要求
    public class A : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBeNull();
            CurrentValue.Value = nameof(A);
        }
    }
}