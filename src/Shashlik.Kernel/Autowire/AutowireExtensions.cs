using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Shashlik.Kernel.Autowire.Attributes;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shashlik.Kernel.Autowire
{
    public static class AutowireExtensions
    {
        /// <summary>
        /// 自动装配options
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IKernelServices AutowireConfigureOptions(this IKernelServices kernelService)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
              .GetMethod("Configure", new Type[] { typeof(IServiceCollection), typeof(IConfiguration) });

            kernelService.BeginAutowireService<AutoOptionsAttribute>()
                .Build(
                    r =>
                    {
                        // 注册options
                        if (r.ServiceType.IsAbstract)
                            return;
                        var instance = r.ServiceInstance as AutoOptionsAttribute;

                        // invoke: Configure<TOptions>(this IServiceCollection services, IConfiguration config)
                        method.MakeGenericMethod(r.ServiceType)
                        .Invoke(null, new object[] { kernelService.Services, kernelService.RootConfiguration.GetSection(instance.Section) });
                    });

            return kernelService;
        }

        /// <summary>
        /// 自动装配 <see cref="IAutowireConfigureServices"/>服务配置
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static IKernelServices AutowireConfigureService(this IKernelServices kernelService)
        {
            return kernelService.BeginAutowireConfigureService()
                    .BuildAutowireConfigureService();
        }

        /// <summary>
        /// 开始自动装配IAutoService
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireServiceBuilder BeginAutowireConfigureService(this IKernelServices kernelService)
        {
            return kernelService.BeginAutowireService<IAutowireConfigureServices>();
        }

        /// <summary>
        /// 构建自动服务装配
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelServices BuildAutowireConfigureService(this IAutowireServiceBuilder builder)
        {
            if (builder.AutowireBaseType != typeof(IAutowireConfigureServices))
                throw new Exception($"error auto service type, must be {typeof(IAutowireConfigureServices)}.");

            builder.Build(r => ((IAutowireConfigureServices)r.ServiceInstance).ConfigureServices(builder.KernelService));
            return builder.KernelService;
        }

        /// <summary>
        /// 自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        public static IKernelConfigure AutowireConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowireConfigure()
                .BuildAutowireConfigure();
        }

        /// <summary>
        /// 开始自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireConfigureBuilder BeginAutowireConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowireConfigure<IAutowireConfigure>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutowireConfigure(this IAutowireConfigureBuilder builder)
        {
            if (builder.AutowireBaseType != typeof(IAutowireConfigure))
                throw new Exception($"error auto configure type, must be {typeof(IAutowireConfigure)}.");
            return builder.Build(r => ((IAutowireConfigure)r.ServiceInstance).Configure(builder.KernelConfigure));
        }
    }
}
