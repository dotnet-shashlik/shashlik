using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    public static class IAutowireBuilderExtensions
    {

        /// <summary>
        /// 开始自动装配服务
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireServiceBuilder BeginAutowireService<TBaseType>(this IKernelService kernelService)
            where TBaseType : class
        {
            using var serviceProvider = kernelService.Services.BuildServiceProvider();
            var autoInitializer = serviceProvider.GetRequiredService<IAutowireProvider>();
            var builder = new DefaultAutowireServiceBuilder(typeof(TBaseType).GetTypeInfo(), autoInitializer,
                kernelService.ScanFromDependencyContext, kernelService.RootConfiguration, kernelService);
            builder.DependencyContext = kernelService.ScanFromDependencyContext;
            return builder;
        }

        /// <summary>
        /// 开始自动配置项目
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireConfigureBuilder BeginAutowireConfigure<TBaseType>(this IKernelConfigure kernelConfigure)
            where TBaseType : class
        {
            var autoInitializer = kernelConfigure.ServiceProvider.GetRequiredService<IAutowireProvider>();
            var kernelService = kernelConfigure.ServiceProvider.GetService<IKernelService>();
            var builder = new DefaultAutowireConfigureBuilder(typeof(TBaseType).GetTypeInfo(), autoInitializer,
                kernelService.ScanFromDependencyContext, kernelService.RootConfiguration, kernelConfigure);

            return builder;
        }

        /// <summary>
        /// 执行自动装配构建
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initBeforeAction"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelService Build(this IAutowireServiceBuilder builder, Action<AutowireDescriptor> autowireAction)
        {
            IDictionary<TypeInfo, AutowireDescriptor> descriptors;
            if (!builder.AutowireBaseTypeIsAttribute)
                descriptors = builder.AutowireProvider.LoadFrom(builder.AutowireBaseType, builder.KernelService.Services,
                    builder.Removes, builder.DependencyContext);
            else
                descriptors = builder.AutowireProvider.LoadFromAttribute(builder.AutowireBaseType, builder.DependencyContext);

            if (autowireAction != null)
                builder.AutowireProvider.Autowire(descriptors, autowireAction);
            return builder.KernelService;
        }

        /// <summary>
        /// 执行自动装配构建
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="autowireAction"></param>
        /// <returns></returns>
        public static IKernelConfigure Build(this IAutowireConfigureBuilder builder, Action<AutowireDescriptor> autowireAction)
        {
            IDictionary<TypeInfo, AutowireDescriptor> descriptors;
            if (!builder.AutowireBaseTypeIsAttribute)
                descriptors = builder.AutowireProvider.LoadFrom(builder.AutowireBaseType, builder.KernelConfigure.ServiceProvider,
                    builder.Removes, builder.DependencyContext);
            else
                descriptors = builder.AutowireProvider.LoadFromAttribute(builder.AutowireBaseType, builder.DependencyContext);

            if (autowireAction != null)
                builder.AutowireProvider.Autowire(descriptors, autowireAction);
            return builder.KernelConfigure;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <typeparam name="TMatchType"></typeparam>
        /// <param name="automaticBuilder"></param>
        /// <returns></returns>
        public static T Remove<TMatchType, T>(this T automaticBuilder)
            where T : IAutowireBuilder
        {
            automaticBuilder.Removes.Add(typeof(TMatchType).GetTypeInfo());
            return automaticBuilder;
        }
    }
}
