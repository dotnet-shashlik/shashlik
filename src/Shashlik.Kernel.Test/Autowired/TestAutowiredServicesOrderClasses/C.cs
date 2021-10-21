using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesOrderClasses
{
    [Order(2)]
    [BeforeAt(typeof(D))]
    [Transient]
    public class C : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(B));
            CurrentValue.Value = nameof(C);
        }
    }
}