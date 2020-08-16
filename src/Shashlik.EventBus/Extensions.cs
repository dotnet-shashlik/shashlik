using Microsoft.Extensions.DependencyInjection;
using System;
using Shashlik.Kernel;
using DotNetCore.CAP;
using DotNetCore.CAP.Internal;

namespace Shashlik.EventBus
{
    public static class Extensions
    {
        /// <summary>
        /// 增加Shashlik 事件总线
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelBuilder AddEventBus(this IKernelBuilder kernelBuilder, Action<CapOptions> capAction)
        {
            kernelBuilder.Services.AddCap(capAction);
            // 替换cap默认的消费者服务查找器
            kernelBuilder.Services.AddSingleton<IConsumerServiceSelector, ShashlikConsumerServiceSelector>();
            return kernelBuilder;
        }
    }
}
