using DotNetCore.CAP.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowire;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    public class EventBusAutowireService : IAutowireConfigureService
    {
        public EventBusAutowireService(IOptions<EventBusOptions> eventBusOptions)
        {
            EventBusOptions = eventBusOptions.Value;
        }

        EventBusOptions EventBusOptions { get; }

        public void ConfigureServices(IKernelService kernelService)
        {
            kernelService.Services.AddCap(r =>
            {
                r.Version = EventBusOptions.Version;
                r.ConsumerThreadCount = EventBusOptions.ConsumerThreadCount;
                r.DefaultGroup = EventBusOptions.DefaultGroup;
                r.FailedRetryCount = EventBusOptions.FailedRetryCount;
                r.FailedRetryInterval = EventBusOptions.FailedRetryInterval;
                r.SucceedMessageExpiredAfter = EventBusOptions.SucceedMessageExpiredAfter;

                kernelService
                    .BeginAutowireService<IEventBusConfigureService>()
                    .Build(descriptor =>
                       {
                           (descriptor.ServiceInstance as IEventBusConfigureService)
                               .Configure(r);
                       });
            });
        }
    }
}
