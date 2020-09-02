using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Autowire.Attributes;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesClasses
{
    [BeforeAt(typeof(D))]
    public class C : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(B));
            CurrentValue.Value = nameof(C);
        }
    }
}