using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredServices : IAssembler
    {
        void ConfigureServices(IServiceCollection serviceCollection);
    }
}