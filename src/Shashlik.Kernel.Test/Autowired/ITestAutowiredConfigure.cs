using System;

namespace Shashlik.Kernel.Test.Autowired
{
    public interface ITestAutowiredConfigure
    {
        void Configure(IServiceProvider serviceProvider);
    }
}