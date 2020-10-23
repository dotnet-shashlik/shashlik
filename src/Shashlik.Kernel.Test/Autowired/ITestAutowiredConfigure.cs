using System;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredConfigure : IAutowire
    {
        void Configure(IServiceProvider serviceProvider);
    }
}