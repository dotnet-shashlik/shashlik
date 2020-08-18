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
        public static IKernelService AutoOptions(
            this IKernelService kernelService,
            IConfiguration rootConfiguration)
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

                        // Configure<TOptions>(this IServiceCollection services, IConfiguration config)
                        method.MakeGenericMethod(r.ServiceType).Invoke(null, new object[] { kernelService.Services, rootConfiguration.GetSection(instance.Section) });
                    },
                    r => { });

            return kernelService;
        }

        /// <summary>
        /// 自动装配AutoService
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static IKernelService AutoService(
            this IKernelService kernelService,
            IConfiguration rootConfiguration)
        {
            return kernelService.BeginAutoService()
                    .BuildAutoService(rootConfiguration);
        }

        /// <summary>
        /// 开始自动装配IAutoService
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireServiceBuilder BeginAutoService(
            this IKernelService kernelService)
        {
            return kernelService.BeginAutowireService<IAutowireConfigureServices>();
        }

        /// <summary>
        /// 构建自动服务装配
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelService BuildAutoService(this IAutowireServiceBuilder builder, IConfiguration rootConfiguration)
        {
            if (builder.AutowireBaseType != typeof(IAutowireConfigureServices))
                throw new Exception($"error auto service type, must be {typeof(IAutowireConfigureServices)}.");

            builder.Build(
            r =>
                {
                    builder.KernelService.Services.TryAddSingleton(r.ServiceType, r.ServiceType);
                },
            r =>
                {
                    using var serviceProvider = builder.KernelService.Services.BuildServiceProvider();
                    var serviceInstance = serviceProvider.GetService(r.ServiceType);
                    var instance = serviceInstance as IAutowireConfigureServices;
                    // IAutoServices类型
                    instance.ConfigureServices(builder.KernelService, rootConfiguration);
                });
            return builder.KernelService;
        }

        /// <summary>
        /// 自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        public static IKernelConfigure AutoConfire(
            this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutoConfigure()
                .BuildAutoConfigure();
        }

        /// <summary>
        /// 开始自动配置IAutoConfigure
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireConfigureBuilder BeginAutoConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowireConfigure<IAutowireConfigure>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutoConfigure(this IAutowireConfigureBuilder builder)
        {
            if (builder.AutowireBaseType != typeof(IAutowireConfigure))
                throw new Exception($"error auto configure type, must be {typeof(IAutowireConfigure)}.");
            return builder.Build(null, r =>
             {
                 (r.ServiceInstance as IAutowireConfigure).Configure(builder.KernelConfigure);
             });
        }
    }
}
