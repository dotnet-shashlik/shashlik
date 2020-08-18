using Microsoft.Extensions.DependencyInjection;
using System;
using Shashlik.Kernel;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;
using Shashlik.Kernel.Autowire;

namespace Shashlik.EventBus
{
    public static class Extensions
    {
        /// <summary>
        /// 增加Shashlik 事件总线
        /// </summary>
        /// <param name="kernelService"></param>
        /// <returns></returns>
        public static IKernelService AddEventBus(this IKernelService kernelService, Action<CapOptions> capAction)
        {
            kernelService.Services.AddCap(capAction);

            kernelService.BeginAutowireService<IEventBusAutowireService>()
                .BuildAutoService(null)

            // 替换cap默认的消费者服务查找器
            kernelService.Services.AddSingleton<IConsumerServiceSelector, ShashlikConsumerServiceSelector>();
            return kernelService;
        }
    }
}
