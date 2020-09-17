using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Autowired.Attributes;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesClasses
{
    [BeforeAt(typeof(C))]
    public class B : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(A));
            CurrentValue.Value = nameof(B);
        }
    }
}