using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesOrderClasses
{
    [Order(3)]
    [BeforeAt(typeof(C))]
    public class B : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(null);
            CurrentValue.Value = nameof(B);
        }
    }
}