using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.Captcha.DistributedCache.Tests
{
    public class TestConfigure : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddDataProtection();
        }
    }
}