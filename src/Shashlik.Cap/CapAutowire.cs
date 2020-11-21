using System.Linq;
using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Cap
{
    /// <summary>
    /// event bus自动装配,装配顺序600
    /// </summary>
    [Order(600)]
    public class CapAutowire : IServiceAutowire
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
                if (CapOptions.Version != null)
                    r.Version = CapOptions.Version;
                if (CapOptions.ConsumerThreadCount.HasValue)
                    r.ConsumerThreadCount = CapOptions.ConsumerThreadCount.Value;
                if (CapOptions.DefaultGroup != null)
                    r.DefaultGroup = CapOptions.DefaultGroup;
                if (CapOptions.FailedRetryCount.HasValue)
                    r.FailedRetryCount = CapOptions.FailedRetryCount.Value;
                if (CapOptions.FailedRetryInterval.HasValue)
                    r.FailedRetryInterval = CapOptions.FailedRetryInterval.Value;
                if (CapOptions.SucceedMessageExpiredAfter.HasValue)
                    r.SucceedMessageExpiredAfter = CapOptions.SucceedMessageExpiredAfter.Value;

                kernelService.Autowire<ICapAutowire>(a => a.Configure(r));
            });

            var exists = kernelService.Services.FirstOrDefault(r => r.ServiceType == typeof(IConsumerServiceSelector));
            if (exists != null)
                kernelService.Services.Remove(exists);

            kernelService.Services.Add(ServiceDescriptor.Describe(typeof(IConsumerServiceSelector),
                typeof(CapConsumerServiceSelector), ServiceLifetime.Singleton));
        }
    }
}