using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesOrderClasses
{
    [Order(1)]
    [AfterAt(typeof(C))]
    public class D : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(C));
            CurrentValue.Value = nameof(D);
        }
    }
}