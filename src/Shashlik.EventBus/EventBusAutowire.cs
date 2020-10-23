using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RestSharp.Extensions;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Autowired;

namespace Shashlik.EventBus
{
    /// <summary>
    /// event bus自动装配
    /// </summary>
    public class EventBusAutowire : IServiceAutowire
    {
        public EventBusAutowire(IOptions<EventBusOptions> eventBusOptions)
        {
            EventBusOptions = eventBusOptions.Value;
        }

        private EventBusOptions EventBusOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!EventBusOptions.Enable)
                return;

            kernelService.Services.AddCap(r =>
            {
                if (EventBusOptions.Version != null)
                    r.Version = EventBusOptions.Version;
                if (EventBusOptions.ConsumerThreadCount.HasValue)
                    r.ConsumerThreadCount = EventBusOptions.ConsumerThreadCount.Value;
                if (EventBusOptions.DefaultGroup != null)
                    r.DefaultGroup = EventBusOptions.DefaultGroup;
                if (EventBusOptions.FailedRetryCount.HasValue)
                    r.FailedRetryCount = EventBusOptions.FailedRetryCount.Value;
                if (EventBusOptions.FailedRetryInterval.HasValue)
                    r.FailedRetryInterval = EventBusOptions.FailedRetryInterval.Value;
                if (EventBusOptions.SucceedMessageExpiredAfter.HasValue)
                    r.SucceedMessageExpiredAfter = EventBusOptions.SucceedMessageExpiredAfter.Value;

                kernelService.Autowire<IEventBusAutowire>(a => a.Configure(r));
            });

            kernelService.Services.Replace(ServiceDescriptor.Describe(typeof(IConsumerServiceSelector),
                typeof(EventBusConsumerServiceSelector), ServiceLifetime.Singleton));
        }
    }
}