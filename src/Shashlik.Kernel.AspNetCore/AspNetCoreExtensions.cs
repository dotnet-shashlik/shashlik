using Shashlik.Kernel.Dependency;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Autowire;
using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel
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
        /// <param name="rootConfiguration"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        public static IKernelConfigure AutoAspNetConfigure(
            this IKernelConfigure kernelConfigure,
            IApplicationBuilder app)
        {
            return kernelConfigure.BeginAutoAspNetConfigure()
                .BuildAutoAspNetConfigure(app);
        }

        /// <summary>
        /// 开始自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireConfigureBuilder BeginAutoAspNetConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowireConfigure<IAutowireConfigureAspNet>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutoAspNetConfigure(this IAutowireConfigureBuilder builder, IApplicationBuilder app)
        {
            if (builder.AutowireBaseType != typeof(IAutowireConfigureAspNet))
                throw new Exception($"error auto configure type, must be {typeof(IAutowireConfigureAspNet)}.");
            return builder.Build(r =>
           {
               (r.ServiceInstance as IAutowireConfigureAspNet).Configure(app);
           });
        }
    }
}
