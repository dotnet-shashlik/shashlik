using Shashlik.Kernel.Dependency;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Autowire.Attributes;
using Shashlik.Kernel.Autowired;
using Shashlik.Kernel.Locker;
using Shashlik.Kernel.Locker.Memory;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 内核扩展类
    /// </summary>
    public static class KernelExtensions
    {
        /// <summary>
        /// AddShashlik
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration">根配置</param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <param name="autowiredOptions">是否自动配置options:<see cref="AutoOptionsAttribute"/></param>
        /// <param name="autowiredConfigureService">是否自动装配 服务配置:<see cref="IAutowiredConfigureServices"/></param>
        /// <returns></returns>
        public static IKernelServices AddShashlik(
            this IServiceCollection services,
            IConfiguration rootConfiguration,
            DependencyContext dependencyContext = null,
            bool autowiredOptions = true,
            bool autowiredConfigureService = true
        )
        {
            dependencyContext ??= DependencyContext.Default;

            // 查找所有包含Shashlik.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies = AssemblyHelper.GetReferredAssemblies<IKernelServices>(dependencyContext);
            conventionAssemblies.Add(typeof(IKernelServices).Assembly);
            var kernelService = services.AddShashlik(rootConfiguration, conventionAssemblies, dependencyContext);
            if (autowiredOptions)
                kernelService.AutowiredOptions();
            if (autowiredConfigureService)
                kernelService.AutowiredServices();

            return kernelService;
        }

        /// <summary>
        /// AddShashlik
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="assemblies"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static IKernelServices AddShashlik(
            this IServiceCollection services,
            IConfiguration rootConfiguration,
            IEnumerable<Assembly> assemblies,
            DependencyContext dependencyContext = null)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            var kernelService = new KernelService(services, dependencyContext, rootConfiguration);

            services.TryAddSingleton<ILock, MemoryLock>();
            services.AddSingleton<IKernelServices>(kernelService);

            services
                .TryAddSingleton<IConventionServiceDescriptorProvider, DefaultConventionServiceDescriptorProvider>();
            services.TryAddSingleton<IBasedOnServiceDescriptorProvider, DefaultBasedOnServiceDescriptorProvider>();
            services.TryAddSingleton<IConditionFilterAddProvider, DefaultConditionFilterAddProvider>();
            services.TryAddSingleton<IAutowiredProvider>(new DefaultAutowiredProvider());

            using var serviceProvider = services.BuildServiceProvider();
            var conventionServiceDescriptorProvider =
                serviceProvider.GetService<IConventionServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            foreach (var item in assemblies)
            {
                var serviceDescriptors = conventionServiceDescriptorProvider.FromAssembly(item);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, services, rootConfiguration, hostEnvironment);
            }

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
            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            List<Assembly> assemblies =
                AssemblyHelper.GetReferredAssemblies<IKernelServices>(kernelService.ScanFromDependencyContext);
            assemblies.Add(typeof(IKernelServices).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors =
                    basedOnServiceDescriptorProvider.FromAssembly(item, typeof(TBaseType).GetTypeInfo(),
                        serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelService.Services,
                    kernelService.RootConfiguration, hostEnvironment);
            }

            return kernelService;
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
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            var assemblies =
                AssemblyHelper.GetReferredAssemblies<IKernelServices>(kernelService.ScanFromDependencyContext);
            assemblies.Add(typeof(IKernelServices).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, baseType, serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelService.Services,
                    kernelService.RootConfiguration, hostEnvironment);
            }

            return kernelService;
        }

        /// <summary>
        /// Shashlik kernel 配置,执行自动装配 应用配置:<see cref="IAutowiredConfigure"/>
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="autowiredConfigure"></param>
        /// <returns></returns>
        public static IKernelConfigure UseShashlik(this IServiceProvider serviceProvider,
            bool autowiredConfigure = true)
        {
            KernelServiceProvider.InitServiceProvider(serviceProvider);
            var kernelConfigure = new KernelConfigure(serviceProvider);
            if (autowiredConfigure)
                kernelConfigure.AutowiredConfigure();
            return kernelConfigure;
        }
    }
}