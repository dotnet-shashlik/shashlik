using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Shashlik.Kernel
{
    internal class DefaultInitializedKernelService : IInitializedKernelService
    {
        public DefaultInitializedKernelService(IKernelServices kernelServices)
        {
            KernelServices = kernelServices;
        }

        private IKernelServices KernelServices { get; }
        public IServiceCollection Services => KernelServices.Services;
        public DependencyContext ScanFromDependencyContext => KernelServices.ScanFromDependencyContext;
        public IConfiguration RootConfiguration => KernelServices.RootConfiguration;
    }
}