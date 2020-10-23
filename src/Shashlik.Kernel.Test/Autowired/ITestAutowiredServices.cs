using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredServices : IAutowire
    {
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}