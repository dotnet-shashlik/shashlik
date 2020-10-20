using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RestSharp.Extensions;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.EventBus
{
    /// <summary>
    /// event bus自动装配
    /// </summary>
    public class EventBusConfigure : IAutowiredConfigureServices
    {
        public EventBusConfigure(IOptions<EventBusOptions> eventBusOptions)
        {
            EventBusOptions = eventBusOptions.Value;
        }

        EventBusOptions EventBusOptions { get; }

        public void ConfigureServices(IKernelServices kernelService)
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
                kernelService
                    .BeginAutowired<IEventBusConfigure>()
                    .Build(descriptor => { (descriptor.ServiceInstance as IEventBusConfigure)!.Configure(r); });
            });
            kernelService.Services.Replace(ServiceDescriptor.Describe(typeof(IConsumerServiceSelector),
                typeof(EventBusConsumerServiceSelector), ServiceLifetime.Singleton));
        }
    }
}