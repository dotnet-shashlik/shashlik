using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel.Autowired.Attributes;

// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.Kernel.Autowired
{
    public static class AutowiredExtensions
    {
        /// <summary>
        /// 自动装配options,使用<see cref="AutoOptionsAttribute"/>特性的类
        /// </summary>
        /// <param name="kernelService"></param>
        /// <returns></returns>
        public static IKernelServices AutowiredOptions(this IKernelServices kernelService)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new Type[] {typeof(IServiceCollection), typeof(IConfiguration)});
            if (method == null)
                throw new KernelExceptionInitException(
                    $"cannot find method: OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(this IServiceCollection services, IConfiguration config).");

            kernelService.BeginAutowired<AutoOptionsAttribute>()
                .Build(r =>
                {
                    // 注册options
                    if (r.ServiceType.IsAbstract)
                        return;
                    var instance = r.Attribute as AutoOptionsAttribute;

                    // invoke: Configure<TOptions>(this IServiceCollection services, IConfiguration config)
                    method.MakeGenericMethod(r.ServiceType)
                        .Invoke(null, new object[]
                        {
                            kernelService.Services, kernelService.RootConfiguration.GetSection(instance!.Section)
                        });
                });

            return kernelService;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IAutowiredConfigureServices"/>
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IKernelServices AutowiredServices(this IKernelServices kernelServices)
        {
            return kernelServices.BeginAutowiredServices()
                .BuildAutowiredServices();
        }

        /// <summary>
        /// 开始自动装配IAutoService
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IAutowiredServiceBuilder BeginAutowiredServices(this IKernelServices kernelServices)
        {
            return kernelServices.BeginAutowired<IAutowiredConfigureServices>();
        }

        /// <summary>
        /// 构建自动服务装配
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelServices BuildAutowiredServices(this IAutowiredServiceBuilder builder)
        {
            if (builder.AutowiredBaseType != typeof(IAutowiredConfigureServices))
                throw new Exception($"error auto service type, must be {typeof(IAutowiredConfigureServices)}.");

            builder.Build(r =>
                ((IAutowiredConfigureServices) r.ServiceInstance)!.ConfigureServices(builder.KernelService));
            return builder.KernelService;
        }

        /// <summary>
        /// 自动装配应用配置:<see cref="IAutowiredConfigure"/>
        /// </summary>
        /// <param name="kernelConfigure"></param>
        public static IKernelConfigure AutowiredConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowiredConfigure()
                .BuildAutowiredConfigure();
        }

        /// <summary>
        /// 开始自动配置<see cref="IAutowiredConfigure"/>
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <returns></returns>
        public static IAutowiredConfigureBuilder BeginAutowiredConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowired<IAutowiredConfigure>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutowiredConfigure(this IAutowiredConfigureBuilder builder)
        {
            if (builder.AutowiredBaseType != typeof(IAutowiredConfigure))
                throw new Exception($"error auto configure type, must be {typeof(IAutowiredConfigure)}.");
            return builder.Build(r => ((IAutowiredConfigure) r.ServiceInstance)!.Configure(builder.KernelConfigure));
        }
    }
}