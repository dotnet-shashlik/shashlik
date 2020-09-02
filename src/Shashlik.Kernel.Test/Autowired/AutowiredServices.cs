using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Autowired;
using Shashlik.Kernel.Test.Options;

namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredServices : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.Services.Configure<TestOptions3>(r => { r.Name = "张三"; });
        }
    }
}