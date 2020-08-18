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
    public class EventBusAutowireServices : IAutowireConfigureServices
    {
        public EventBusAutowireServices(IOptions<EventBusOptions> eventBusOptions)
        {
            EventBusOptions = eventBusOptions.Value;
        }

        EventBusOptions EventBusOptions { get; }

        public void ConfigureServices(IKernelServices kernelService)
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
                    .BeginAutowireService<IEventBusConfigureServices>()
                    .Build(descriptor =>
                       {
                           (descriptor.ServiceInstance as IEventBusConfigureServices)
                               .Configure(r);
                       });
            });
        }
    }
}
