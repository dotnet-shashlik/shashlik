using System;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredConfigure : IAssembler
    {
        void Configure(IServiceProvider serviceProvider);
    }
}