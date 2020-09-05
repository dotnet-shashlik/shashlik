using System;
using Microsoft.AspNetCore.Builder;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Kernel.AspNetCore
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class AspNetCoreExtensions
    {
        /// <summary>
        /// 自动配置IAutoAspNetConfigure
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <param name="app"></param>
        public static IKernelConfigure AutowiredAspNetConfigure(
            this IKernelConfigure kernelConfigure,
            IApplicationBuilder app)
        {
            return kernelConfigure.BeginAutowiredAspNetConfigure()
                .BuildAutowiredAspNetConfigure(app);
        }

        /// <summary>
        /// 开始自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <returns></returns>
        public static IAutowiredConfigureBuilder BeginAutowiredAspNetConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowired<IAutowiredConfigureAspNetCore>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutowiredAspNetConfigure(this IAutowiredConfigureBuilder builder, IApplicationBuilder app)
        {
            if (builder.AutowiredBaseType != typeof(IAutowiredConfigureAspNetCore))
                throw new Exception($"error auto configure type, must be {typeof(IAutowiredConfigureAspNetCore)}.");
            return builder.Build(r =>
           {
               (r.ServiceInstance as IAutowiredConfigureAspNetCore)?.Configure(app);
           });
        }
    }
}
