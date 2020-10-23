using Shashlik.Kernel;

namespace Shashlik.Mapper
{
    public class AutoMapperAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddAutoMapperByConvention();
        }
    }
}