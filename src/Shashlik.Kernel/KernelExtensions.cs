using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Shashlik.Kernel.Autowire;

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
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <returns></returns>
        public static IKernelService AddShashlik(this IServiceCollection services, IConfiguration rootConfiguration, DependencyContext dependencyContext)
        {
            if (dependencyContext == null)
                throw new ArgumentNullException(nameof(dependencyContext));

            // 查找所有包含Shashlik.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies = AssemblyHelper.GetReferredAssemblies<IKernelService>(dependencyContext);
            conventionAssemblies.Add(typeof(IKernelService).Assembly);
            return services.AddShashlik(rootConfiguration, conventionAssemblies, dependencyContext);
        }

        /// <summary>
        /// AddShashlik
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>
        /// <returns></returns>
        static IKernelService AddShashlik(this IServiceCollection services, IConfiguration rootConfiguration, IEnumerable<Assembly> assemblies, DependencyContext dependencyContext)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));


            services.TryAddSingleton<IConventionServiceDescriptorProvider, DefaultConventionServiceDescriptorProvider>();
            services.TryAddSingleton<IBasedOnServiceDescriptorProvider, DefaultBasedOnServiceDescriptorProvider>();
            services.TryAddSingleton<IConditionFilterAddProvider, DefaultConditionFilterAddProvider>();
            services.TryAddSingleton<IAutowireInitializer>(new DefaultAutowireInitializer());


            using var serviceProvider = services.BuildServiceProvider();
            var conventionServiceDescriptorProvider = serviceProvider.GetService<IConventionServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            foreach (var item in assemblies)
            {
                var serviceDescriptors = conventionServiceDescriptorProvider.FromAssembly(item);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, services, rootConfiguration, hostEnvironment);
            }

            var kernelService = new KernelService(services, dependencyContext, rootConfiguration);
            services.AddSingleton(kernelService);
            return kernelService;
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TBaseType"/>的子类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static IKernelService AddServiceByBasedOn<TBaseType>(this IKernelService kernelService,
            ServiceLifetime serviceLifetime)
        {
            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            List<Assembly> assemblies = AssemblyHelper.GetReferredAssemblies<IKernelService>(kernelService.ScanFromDependencyContext);
            assemblies.Add(typeof(IKernelService).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, typeof(TBaseType).GetTypeInfo(), serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelService.Services, kernelService.RootConfiguration, hostEnvironment);
            }

            return kernelService;
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TBaseType"/>的子类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static IKernelService AddServiceByBasedOn(this IKernelService kernelService, TypeInfo baseType,
            ServiceLifetime serviceLifetime)
        {
            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            List<Assembly> assemblies = AssemblyHelper.GetReferredAssemblies<IKernelService>(kernelService.ScanFromDependencyContext);
            assemblies.Add(typeof(IKernelService).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, baseType, serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelService.Services, kernelService.RootConfiguration, hostEnvironment);
            }

            return kernelService;
        }

        /// <summary>
        /// Shashlik kernel 配置
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static IKernelConfigure UseShashlik(this IServiceProvider serviceProvider)
        {
            KernelServiceProvider.InitServiceProvider(serviceProvider);
            return new KernelConfigure(serviceProvider);
        }
    }
}
