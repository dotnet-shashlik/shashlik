using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.Captcha.DistributedCache.Tests
{
    public class TestConfigure : IServiceAssembler
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddDataProtection();
        }
    }
}