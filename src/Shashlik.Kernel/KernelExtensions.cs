using Shashlik.Kernel.Dependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Assembler;
using Shashlik.Kernel.Options;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// AddShashlik,执行典型的shashlik服务配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration">根配置</param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <returns></returns>
        public static IServiceCollection AddShashlik(
            this IServiceCollection services,
            IConfiguration rootConfiguration)
        {
            return services.AddShashlikCore(rootConfiguration)
                // 配置装载
                .AssembleOptions()
                // 注册约定的服务
                .RegistryConventionServices()
                // 自动服务装配
                .AssembleServices()
                // 执行服务过滤
                .DoFilter();
        }

        /// <summary>
        /// AddShashlikCore 注册核心服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration">根配置</param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <returns></returns>
        public static IKernelServices AddShashlikCore(
            this IServiceCollection services,
            IConfiguration rootConfiguration
        )
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (rootConfiguration == null) throw new ArgumentNullException(nameof(rootConfiguration));

            var kernelService = new InnerKernelService(services, DependencyContext.Default, rootConfiguration);
            services.AddSingleton<IKernelServices>(kernelService);

            services.TryAddSingleton<IServiceDescriptorProvider, DefaultServiceDescriptorProvider>();
            services.TryAddSingleton<IFilterProvider, DefaultFilterAddProvider>();
            services.TryAddSingleton(typeof(IAssemblerProvider<>), typeof(DefaultAssemblerProvider<>));
            services.TryAddSingleton(typeof(IAssemblerExecutor<>), typeof(DefaultAssemblerExecutor<>));
            services.TryAddTransient<IOptionsAssembler, DefaultOptionsAssembler>();
            services.AddHostedService<ApplicationLifetimeHostedService>();

            return kernelService;
        }

        /// <summary>
        /// 自动装载所有的配置options,<see cref="AutoOptionsAttribute"/>
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="disableTypes"></param>
        /// <returns></returns>
        public static IKernelServices AssembleOptions(this IKernelServices kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var optionsAutowire = serviceProvider.GetRequiredService<IOptionsAssembler>();
            optionsAutowire.ConfigureAll(kernelServices);
            return kernelServices;
        }

        /// <summary>
        /// 注册约定的服务: Scoped/Singleton/Transient
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IKernelServices RegistryConventionServices(this IKernelServices kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var serviceDescriptorProvider = serviceProvider.GetRequiredService<IServiceDescriptorProvider>();

            var types
                = ReflectionHelper.GetTypesAndAttribute<ServiceAttribute>(kernelServices.ScanFromDependencyContext, false);

            foreach (var item in types)
            {
                var services = serviceDescriptorProvider.GetDescriptor(item.Key);
                foreach (var shashlikServiceDescriptor in services)
                {
                    if (!kernelServices.Services.AnyService(shashlikServiceDescriptor.ServiceType, shashlikServiceDescriptor.ImplementationType!))
                        kernelServices.Services.Add(shashlikServiceDescriptor);
                }
            }

            return kernelServices;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceAssembler"/>, **服务装配类所有的条件特性都不可用**
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IKernelServices AssembleServices(this IKernelServices kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var autowireProvider = serviceProvider.GetRequiredService<IAssemblerExecutor<IServiceAssembler>>();
            autowireProvider.Execute(serviceProvider, r => { r.Configure(kernelServices); });
            return kernelServices;
        }

        /// <summary>
        /// 执行条件过滤
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static IServiceCollection DoFilter(this IKernelServices kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var filterProvider = serviceProvider.GetRequiredService<IFilterProvider>();
            var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            // 按条件过滤服务注册
            filterProvider.DoFilter(
                kernelServices.Services,
                kernelServices.RootConfiguration,
                hostEnvironment);

            return kernelServices.Services;
        }

        /// <summary>
        /// Shashlik kernel 配置
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IKernelServiceProvider UseShashlik(this IServiceProvider serviceProvider)
        {
            var kernelServiceProvider = new InnerKernelServiceProvider(serviceProvider);
            GlobalKernelServiceProvider.InitServiceProvider(kernelServiceProvider);
            kernelServiceProvider.AssembleServiceProvider();
            return kernelServiceProvider;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceProviderAssembler"/>
        /// </summary>
        /// <param name="kernelServiceProvider"></param>
        /// <returns></returns>
        public static IKernelServiceProvider AssembleServiceProvider(this IKernelServiceProvider kernelServiceProvider)
        {
            var autowireProvider = kernelServiceProvider.GetRequiredService<IAssemblerProvider<IServiceProviderAssembler>>();
            var kernelServices = kernelServiceProvider.GetRequiredService<IKernelServices>();
            var dic = autowireProvider.Load(kernelServices, kernelServiceProvider);
            autowireProvider.Execute(dic, r => { r.ServiceInstance.Configure(kernelServiceProvider); });
            return kernelServiceProvider;
        }

        /// <summary>
        /// 自定义装配<typeparamref name="T"/>类型
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelServices Assemble<T>(this IKernelServices kernelServices,
            Action<T> autowireAction) where T : IAssembler
        {
            if (autowireAction is null) throw new ArgumentNullException(nameof(autowireAction));
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var autowireProvider = serviceProvider.GetRequiredService<IAssemblerExecutor<T>>();
            autowireProvider.Execute(serviceProvider, autowireAction);
            return kernelServices;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceAssembler"/>
        /// </summary>
        /// <param name="kernelServiceProvider"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelServiceProvider Assemble<T>(this IKernelServiceProvider kernelServiceProvider,
            Action<T> autowireAction) where T : IAssembler
        {
            if (autowireAction is null) throw new ArgumentNullException(nameof(autowireAction));
            var autowireProvider = kernelServiceProvider.GetRequiredService<IAssemblerExecutor<T>>();
            autowireProvider.Execute(kernelServiceProvider, autowireAction);
            return kernelServiceProvider;
        }
    }
}