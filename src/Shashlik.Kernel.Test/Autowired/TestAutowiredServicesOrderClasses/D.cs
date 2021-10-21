using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesOrderClasses
{
    [Order(1)]
    [AfterAt(typeof(C))]
    [Transient]
    public class D : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(C));
            CurrentValue.Value = nameof(D);
        }
    }
}