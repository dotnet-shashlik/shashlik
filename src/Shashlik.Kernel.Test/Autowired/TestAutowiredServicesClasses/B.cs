using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shouldly;

namespace Shashlik.Kernel.Test.Autowired.TestAutowiredServicesClasses
{
    [BeforeAt(typeof(C)), Transient]
    public class B : ITestAutowiredServices
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            CurrentValue.Value.ShouldBe(nameof(A));
            CurrentValue.Value = nameof(B);
        }
    }
}