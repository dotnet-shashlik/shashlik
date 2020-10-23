using Shashlik.Kernel.Dependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Autowire;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// 执行初始化
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public static IInitializedKernelService Initialize(this IKernelServices kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var conventionServiceDescriptorProvider =
                serviceProvider.GetService<IConventionServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            // 查找所有包含Shashlik.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies =
                AssemblyHelper.GetReferredAssemblies<IKernelServices>(kernelServices.ScanFromDependencyContext);
            conventionAssemblies.Add(typeof(IKernelServices).Assembly);

            foreach (var item in conventionAssemblies)
            {
                var serviceDescriptors = conventionServiceDescriptorProvider.FromAssembly(item);
                conditionFilterAddProvider.FilterAndRegistryService(serviceDescriptors, kernelServices.Services,
                    kernelServices.RootConfiguration, hostEnvironment);
            }

            return new DefaultInitializedKernelService(kernelServices);
        }


        /// <summary>
        /// AddShashlik
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration">根配置</param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <returns></returns>
        public static IKernelServices AddShashlik(
            this IServiceCollection services,
            IConfiguration rootConfiguration,
            DependencyContext dependencyContext = null
        )
        {
            dependencyContext ??= DependencyContext.Default;

            var kernelService = new InnerKernelService(services, dependencyContext, rootConfiguration);
            services.AddSingleton<IKernelServices>(kernelService);

            services
                .TryAddSingleton<IConventionServiceDescriptorProvider, DefaultConventionServiceDescriptorProvider>();
            services.TryAddSingleton<IBasedOnServiceDescriptorProvider, DefaultBasedOnServiceDescriptorProvider>();
            services.TryAddSingleton<IConditionProvider, DefaultConditionFilterAddProvider>();
            services.TryAddSingleton(typeof(IAutowireProvider<>), typeof(DefaultAutowireProvider<>));
            services.TryAddSingleton<IOptionsAutowire, DefaultOptionsAutowire>();

            return kernelService;
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TBaseType"/>的子类
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="serviceLifetime"></param>
        public static IKernelServices AddServicesByBasedOn<TBaseType>(this IKernelServices kernelService,
            ServiceLifetime serviceLifetime)
        {
            return AddServicesByBasedOn(kernelService, typeof(TBaseType).GetTypeInfo(), serviceLifetime);
        }

        /// <summary>
        /// 注册程序集中继承自<paramref name="baseType"/>的子类
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="baseType"></param>
        /// <param name="serviceLifetime"></param>
        public static IKernelServices AddServicesByBasedOn(this IKernelServices kernelService, TypeInfo baseType,
            ServiceLifetime serviceLifetime)
        {
            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            var assemblies =
                AssemblyHelper.GetReferredAssemblies<IKernelServices>(kernelService.ScanFromDependencyContext);
            assemblies.Add(typeof(IKernelServices).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, baseType, serviceLifetime);
                conditionFilterAddProvider.FilterAndRegistryService(serviceDescriptors, kernelService.Services,
                    kernelService.RootConfiguration, hostEnvironment);
            }

            return kernelService;
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
            return kernelServiceProvider;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceAutowire"/>
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <returns></returns>
        public static IInitializedKernelService AutowireServices(this IInitializedKernelService kernelServices)
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var autowireProvider = serviceProvider.GetRequiredService<IAutowireProvider<IServiceAutowire>>();
            var dic = autowireProvider.Load(kernelServices, serviceProvider);
            autowireProvider.Autowire(dic, r => { r.ServiceInstance.Configure(kernelServices); });

            return kernelServices;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceProviderAutowire"/>
        /// </summary>
        /// <param name="kernelServiceProvider"></param>
        /// <returns></returns>
        public static IKernelServiceProvider AutowireServiceProvider(this IKernelServiceProvider kernelServiceProvider)
        {
            var autowireProvider =
                kernelServiceProvider.GetRequiredService<IAutowireProvider<IServiceProviderAutowire>>();
            var kernelServices = kernelServiceProvider.GetRequiredService<IKernelServices>();
            var dic = autowireProvider.Load(kernelServices, kernelServiceProvider);
            autowireProvider.Autowire(dic, r => { r.ServiceInstance.Configure(kernelServiceProvider); });
            return kernelServiceProvider;
        }

        /// <summary>
        /// 自动装载所有的配置options,<see cref="AutoOptionsAttribute"/>
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="disableTypes"></param>
        /// <returns></returns>
        public static T AutowireOptions<T>(this T kernelServices,
            params Type[] disableTypes) where T : IKernelServices
        {
            var optionsAutowire = new DefaultOptionsAutowire();
            foreach (var disableType in disableTypes)
                optionsAutowire.Disabled.Add(disableType);

            return (T) optionsAutowire.ConfigureAll(kernelServices);
        }

        /// <summary>
        /// 自定义装配<typeparamref name="T"/>类型
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelServices Autowire<T>(this IKernelServices kernelServices,
            Action<T> autowireAction) where T : IAutowire
        {
            if (autowireAction == null) throw new ArgumentNullException(nameof(autowireAction));
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var autowireProvider = serviceProvider.GetRequiredService<IAutowireProvider<T>>();
            var dic = autowireProvider.Load(kernelServices, serviceProvider);
            autowireProvider.Autowire(dic, r => { autowireAction(r.ServiceInstance); });
            return kernelServices;
        }

        /// <summary>
        /// 自动装配服务配置: <see cref="IServiceAutowire"/>
        /// </summary>
        /// <param name="kernelServiceProvider"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelServiceProvider Autowire<T>(this IKernelServiceProvider kernelServiceProvider,
            Action<T> autowireAction) where T : IAutowire
        {
            if (autowireAction == null) throw new ArgumentNullException(nameof(autowireAction));
            var autowireProvider = kernelServiceProvider.GetRequiredService<IAutowireProvider<T>>();
            var kernelServices = kernelServiceProvider.GetService<IKernelServices>();
            var dic = autowireProvider.Load(kernelServices, kernelServiceProvider);
            autowireProvider.Autowire(dic, r => { autowireAction(r.ServiceInstance); });
            return kernelServiceProvider;
        }
    }
}