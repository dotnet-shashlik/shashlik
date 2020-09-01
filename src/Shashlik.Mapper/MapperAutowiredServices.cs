using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Mapper
{
    public class MapperAutowiredServices : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddAutoMapperByConvention();
        }
    }
}