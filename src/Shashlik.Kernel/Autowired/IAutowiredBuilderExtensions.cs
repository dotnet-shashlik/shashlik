using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace Shashlik.Kernel.Autowired
{
    public static class IAutowiredBuilderExtensions
    {
        /**
         * 为扩展做预留,暂时不考虑替换/删除等,需要业务实践
         * Begin...()
         * ...
         * .Build()
         */
        /// <summary>
        /// 开始自动装配 服务配置
        /// </summary>
        /// <param name="kernelServices"></param>
        /// <typeparam name="TBaseType"></typeparam>
        /// <returns></returns>
        public static IAutowiredServiceBuilder BeginAutowired<TBaseType>(this IKernelServices kernelServices)
            where TBaseType : class
        {
            using var serviceProvider = kernelServices.Services.BuildServiceProvider();
            var autowiredProvider = serviceProvider.GetRequiredService<IAutowiredProvider>();
            return new DefaultAutowiredServiceBuilder(typeof(TBaseType).GetTypeInfo(), autowiredProvider,
                kernelServices.ScanFromDependencyContext, kernelServices);
        }

        /// <summary>
        /// 开始装配 应用配置,<typeparam name="TBaseType"></typeparam>需要自行实现注入,最简单的方式就是继承自:ISingleton/IScoped/ITransient
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <typeparam name="TBaseType"></typeparam>
        /// <returns></returns>
        public static IAutowiredConfigureBuilder BeginAutowired<TBaseType>(
            this IKernelConfigure kernelConfigure)
            where TBaseType : class
        {
            var autoInitializer = kernelConfigure.ServiceProvider.GetRequiredService<IAutowiredProvider>();
            var kernelService = kernelConfigure.ServiceProvider.GetService<IKernelServices>();
            var builder = new DefaultAutowiredConfigureBuilder(typeof(TBaseType).GetTypeInfo(), autoInitializer,
                kernelService.ScanFromDependencyContext, kernelConfigure);

            return builder;
        }

        /// <summary>
        /// 执行自动装配 服务配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="autowiredAction"></param>
        /// <returns></returns>
        public static IKernelServices Build(this IAutowiredServiceBuilder builder,
            Action<AutowiredDescriptor> autowiredAction)
        {
            IDictionary<Type, AutowiredDescriptor> descriptors;
            if (!builder.AutowiredBaseTypeIsAttribute)
                descriptors = builder.AutowiredProvider.LoadFrom(builder.AutowiredBaseType,
                    builder.KernelService.Services, builder.DependencyContext);
            else
                descriptors = builder.AutowiredProvider.LoadFrom(builder.AutowiredBaseType, builder.DependencyContext);

            if (autowiredAction != null)
                builder.AutowiredProvider.Autowired(descriptors, autowiredAction);
            return builder.KernelService;
        }

        /// <summary>
        /// 执行自动装配 应用配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="autowiredAction"></param>
        /// <returns></returns>
        public static IKernelConfigure Build(this IAutowiredConfigureBuilder builder,
            Action<AutowiredDescriptor> autowiredAction)
        {
            IDictionary<Type, AutowiredDescriptor> descriptors;
            if (!builder.AutowiredBaseTypeIsAttribute)
                descriptors = builder.AutowiredProvider.LoadFrom(builder.AutowiredBaseType,
                    builder.KernelConfigure.ServiceProvider);
            else
                descriptors = builder.AutowiredProvider.LoadFrom(builder.AutowiredBaseType, builder.DependencyContext);

            if (autowiredAction != null)
                builder.AutowiredProvider.Autowired(descriptors, autowiredAction);
            return builder.KernelConfigure;
        }
    }
}