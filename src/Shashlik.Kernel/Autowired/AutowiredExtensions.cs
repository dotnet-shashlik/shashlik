using Microsoft.Extensions.DependencyInjection;
using System;
using Shashlik.Kernel.Autowire.Attributes;
using Microsoft.Extensions.Configuration;
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
        public static IKernelServices AutowiredConfigureOptions(this IKernelServices kernelService)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new Type[] {typeof(IServiceCollection), typeof(IConfiguration)});
            if (method == null)
                throw new KernelExceptionInitException(
                    $"cannot find method: OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(this IServiceCollection services, IConfiguration config).");

            kernelService.BeginAutowireService<AutoOptionsAttribute>()
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
                            kernelService.Services, kernelService.RootConfiguration.GetSection(instance.Section)
                        });
                });

            return kernelService;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IAutowiredConfigureServices"/>
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IKernelServices AutowiredConfigureServices(this IKernelServices kernelServices)
        {
            return kernelServices.BeginAutowiredConfigureService()
                .BuildAutowireConfigureServices();
        }

        /// <summary>
        /// 开始自动装配IAutoService
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IAutowiredServiceBuilder BeginAutowiredConfigureService(this IKernelServices kernelServices)
        {
            return kernelServices.BeginAutowireService<IAutowiredConfigureServices>();
        }

        /// <summary>
        /// 构建自动服务装配
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelServices BuildAutowireConfigureServices(this IAutowiredServiceBuilder builder)
        {
            if (builder.AutowireBaseType != typeof(IAutowiredConfigureServices))
                throw new Exception($"error auto service type, must be {typeof(IAutowiredConfigureServices)}.");

            builder.Build(r =>
                ((IAutowiredConfigureServices) r.ServiceInstance).ConfigureServices(builder.KernelService));
            return builder.KernelService;
        }

        /// <summary>
        /// 自动装配应用配置:<see cref="IAutowiredConfigure"/>
        /// </summary>
        /// <param name="kernelConfigure"></param>
        public static IKernelConfigure AutowiredConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowireConfigure()
                .BuildAutowiredConfigure();
        }

        /// <summary>
        /// 开始自动配置<see cref="IAutowiredConfigure"/>
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <returns></returns>
        public static IAutowiredConfigureBuilder BeginAutowireConfigure(this IKernelConfigure kernelConfigure)
        {
            return kernelConfigure.BeginAutowiredConfigure<IAutowiredConfigure>();
        }

        /// <summary>
        /// 构建自动配置IAutoConfigure
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IKernelConfigure BuildAutowiredConfigure(this IAutowiredConfigureBuilder builder)
        {
            if (builder.AutowireBaseType != typeof(IAutowiredConfigure))
                throw new Exception($"error auto configure type, must be {typeof(IAutowiredConfigure)}.");
            return builder.Build(r => ((IAutowiredConfigure) r.ServiceInstance).Configure(builder.KernelConfigure));
        }
    }
}