using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

namespace Shashlik.AutoMapper
{
    /// <summary>
    /// automapper自动装配,装配顺序100
    /// </summary>
    [Order(100)]
    public class AutoMapperAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            kernelService.AddAutoMapperByConvention();
        }
    }
}