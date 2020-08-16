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
        public static IKernelBuilder AddShashlik(this IServiceCollection services, IConfiguration rootConfiguration, DependencyContext dependencyContext = null)
        {
            if (dependencyContext == null)
                throw new ArgumentNullException(nameof(dependencyContext));
            // 查找所有包含Shashlik.Kernel引用的程序集,并按约定进行服务注册
            var conventionAssemblies = AssemblyHelper.GetReferredAssemblies<IKernelBuilder>(dependencyContext);
            conventionAssemblies.Add(typeof(IKernelBuilder).Assembly);
            return services.AddShashlik(rootConfiguration, conventionAssemblies);
        }

        /// <summary>
        /// AddShashlik
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dependencyContext">依赖上下文,null使用默认配置</param>

        /// <returns></returns>
        public static IKernelBuilder AddShashlik(this IServiceCollection services, IConfiguration rootConfiguration, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            services.TryAddSingleton<IConventionServiceDescriptorProvider, DefaultConventionServiceDescriptorProvider>();
            services.TryAddSingleton<IBasedOnServiceDescriptorProvider, DefaultBasedOnServiceDescriptorProvider>();
            services.TryAddSingleton<IConditionFilterAddProvider, DefaultConditionFilterAddProvider>();


            using var serviceProvider = services.BuildServiceProvider();
            var conventionServiceDescriptorProvider = serviceProvider.GetService<IConventionServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            foreach (var item in assemblies)
            {
                var serviceDescriptors = conventionServiceDescriptorProvider.FromAssembly(item);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, services, rootConfiguration, hostEnvironment);
            }

            return new KernelBuilder(services);
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TBaseType"/>的子类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static IKernelBuilder AddServiceByBasedOn<TBaseType>(this IKernelBuilder kernelBuilder,
            ServiceLifetime serviceLifetime, IConfiguration rootConfiguration, IEnumerable<Assembly> assemblies = null)
        {
            using var serviceProvider = kernelBuilder.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, typeof(TBaseType).GetTypeInfo(), serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelBuilder.Services, rootConfiguration, hostEnvironment);
            }

            return kernelBuilder;
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TBaseType"/>的子类
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static IKernelBuilder AddServiceByBasedOn<TBaseType>(this IKernelBuilder kernelBuilder,
            ServiceLifetime serviceLifetime, IConfiguration rootConfiguration, DependencyContext dependencyContext = null)
        {
            using var serviceProvider = kernelBuilder.Services.BuildServiceProvider();
            var basedOnServiceDescriptorProvider = serviceProvider.GetService<IBasedOnServiceDescriptorProvider>();
            var conditionFilterAddProvider = serviceProvider.GetService<IConditionFilterAddProvider>();
            var hostEnvironment = serviceProvider.GetService<IHostEnvironment>();

            List<Assembly> assemblies = AssemblyHelper.GetReferredAssemblies<IKernelBuilder>(dependencyContext);
            assemblies.Add(typeof(IKernelBuilder).Assembly);

            foreach (var item in assemblies)
            {
                var serviceDescriptors = basedOnServiceDescriptorProvider.FromAssembly(item, typeof(TBaseType).GetTypeInfo(), serviceLifetime);
                conditionFilterAddProvider.FilterAdd(serviceDescriptors, kernelBuilder.Services, rootConfiguration, hostEnvironment);
            }

            return kernelBuilder;
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

        /// <summary>
        /// 服务集合是否已经存在<typeparamref name="TType"/>服务类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static bool AnyService<TType>(this IServiceCollection services)
        {
            return services.AnyService(typeof(TType));
        }

        /// <summary>
        /// 服务集合是否已经存在<paramref name="serviceType"/>服务类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static bool AnyService(this IServiceCollection services, Type serviceType)
        {
            return services.Any(r => r.ServiceType == serviceType);
        }
    }
}
