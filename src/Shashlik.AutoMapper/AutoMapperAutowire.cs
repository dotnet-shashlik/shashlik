using Microsoft.Extensions.Options;
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
        public AutoMapperAutowire(IOptions<AutoMapperOptions> options)
        {
            Options = options;
        }

        private IOptions<AutoMapperOptions> Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Value.Enable)
                return;
            kernelService.AddAutoMapperByConvention();
        }
    }
}