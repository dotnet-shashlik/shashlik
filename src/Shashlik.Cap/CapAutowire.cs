using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;

namespace Shashlik.Cap
{
    /// <summary>
    /// event bus自动装配
    /// </summary>
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

            kernelService.Services.Replace(ServiceDescriptor.Describe(typeof(IConsumerServiceSelector),
                typeof(CapConsumerServiceSelector), ServiceLifetime.Singleton));
        }
    }
}