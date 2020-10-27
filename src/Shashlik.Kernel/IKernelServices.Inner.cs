using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Shashlik.Kernel
{
    public class InnerKernelService : IKernelServices
    {
        public InnerKernelService(
            IServiceCollection services,
            DependencyContext scanFromDependencyContext,
            IConfiguration rootConfiguration)
        {
            Services = services;
            ScanFromDependencyContext = scanFromDependencyContext;
            RootConfiguration = rootConfiguration;
        }

        public IServiceCollection Services { get; }
        public DependencyContext ScanFromDependencyContext { get; }
        public IConfiguration RootConfiguration { get; }
    }
}