using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Cap;
using Shashlik.Kernel;

namespace Shashlik.Sms.Cap.Tests
{
    public class CapAutowire : ICapAutowire
    {
        public CapAutowire(IKernelServices kernelServices)
        {
            KernelServices = kernelServices;
        }

        private IKernelServices KernelServices { get; }

        public void Configure(CapOptions capOptions)
        {
            // 内存存储方便测试
            capOptions.UseInMemoryStorage();
        }
    }
}