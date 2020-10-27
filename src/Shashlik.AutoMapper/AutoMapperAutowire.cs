using Shashlik.Kernel;

namespace Shashlik.AutoMapper
{
    public class AutoMapperAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddAutoMapperByConvention();
        }
    }
}