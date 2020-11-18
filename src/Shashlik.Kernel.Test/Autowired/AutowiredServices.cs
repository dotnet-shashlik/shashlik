using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Test.Options;

namespace Shashlik.Kernel.Test.Autowired
{
    public class AutowiredServices : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.Configure<TestOptions3>(r => { r.Name = "张三"; });
        }
    }
}