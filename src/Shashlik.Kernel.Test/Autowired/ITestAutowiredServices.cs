using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredServices
    {
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}