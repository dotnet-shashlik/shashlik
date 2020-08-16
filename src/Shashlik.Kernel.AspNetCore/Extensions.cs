using Shashlik.Kernel.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Shashlik kernel 配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="enableRewind">启用request body重复读</param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig UseShashlikKernelWithAspNetCore(this IApplicationBuilder app, bool enableRewind = true)
        {
            app.UseEnableRequestRewind();
            app.ApplicationServices.UseShashlik();
            return new KernelAspNetCoreConfig(app);
        }

        /// <summary>
        /// then 流式配置
        /// </summary>
        /// <param name="config"></param>
        /// <param name="appConfig"></param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig Then(this IKernelAspNetCoreConfig config, Action<IApplicationBuilder> appConfig)
        {
            appConfig?.Invoke(config.App);
            return config;
        }

        /// <summary>
        /// IKernelConfig as IKernelAspNetCoreConfig
        /// </summary>
        /// <param name="kernelConfig"></param>
        /// <returns></returns>
        public static IKernelAspNetCoreConfig AsKernelAspNetCoreConfig(this IKernelConfig kernelConfig)
        {
            return kernelConfig as IKernelAspNetCoreConfig;
        }
    }
}
