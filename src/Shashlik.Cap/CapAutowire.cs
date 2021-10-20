using System.Linq;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

namespace Shashlik.Cap
{
    /// <summary>
    /// event bus自动装配,装配顺序600
    /// </summary>
    [Order(600)]
    public class CapAutowire : IServiceAssembler
    {
        public CapAutowire(IOptions<ShashlikCapOptions> capOptions)
        {
            CapOptions = capOptions.Value;
        }

        private ShashlikCapOptions CapOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!CapOptions.Enable)
                return;

            kernelService.Services.TryAddSingleton<IEventPublisher, DefaultEventPublisher>();
            kernelService.Services.TryAddSingleton<INameRuler, DefaultNameRuler>();

            kernelService.Services.AddCap(r =>
            {
                CapOptions.CopyTo(r, true);
                kernelService.Assemble<ICapAutowire>(a => a.Configure(r));
            });

            var exists = kernelService.Services.FirstOrDefault(r => r.ServiceType == typeof(IConsumerServiceSelector));
            if (exists != null)
                kernelService.Services.Remove(exists);

            kernelService.Services.Add(ServiceDescriptor.Describe(typeof(IConsumerServiceSelector),
                typeof(CapConsumerServiceSelector), ServiceLifetime.Singleton));
        }
    }
}