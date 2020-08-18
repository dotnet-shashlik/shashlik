﻿using DotNetCore.CAP.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowire;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    public class EventBusAutowireService : IAutowireConfigureServices
    {
        public EventBusAutowireService(EventBusOptions eventBusOptions)
        {
            EventBusOptions = eventBusOptions;
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
