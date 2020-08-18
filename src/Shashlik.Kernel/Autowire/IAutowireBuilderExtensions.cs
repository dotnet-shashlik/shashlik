using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var autoInitializer = serviceProvider.GetRequiredService<IAutowireInitializer>();
            var builder = new DefaultAutowireServiceBuilder(typeof(TBaseType).GetTypeInfo(), autoInitializer, kernelService);
            builder.DependencyContext = kernelService.ScanFromDependencyContext;
            return builder;
        }

        /// <summary>
        /// 开始自动配置
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        /// <returns></returns>
        public static IAutowireConfigureBuilder BeginAutowireConfigure<TBaseType>(this IKernelConfigure kernelConfigure)
            where TBaseType : class
        {
            var autoInitializer = kernelConfigure.ServiceProvider.GetRequiredService<IAutowireInitializer>();
            var builder = new DefaultAutowireConfigureBuilder(typeof(TBaseType).GetTypeInfo(), autoInitializer, kernelConfigure);
            builder.DependencyContext = kernelConfigure.ServiceProvider.GetService<IKernelService>().ScanFromDependencyContext;
            return builder;
        }

        /// <summary>
        /// 执行自动装配构建
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initBeforeAction"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IKernelService Build(this IAutowireServiceBuilder builder, Action<AutowireDescriptor> initBeforeAction, Action<AutowireDescriptor> initAction)
        {
            IDictionary<TypeInfo, AutowireDescriptor> descriptors;
            if (!builder.AutowireBaseTypeIsAttribute)
                descriptors = builder.AutoInitializer.LoadFrom(builder.AutowireBaseType, builder.Replaces, builder.Removes, builder.DependencyContext);
            else
                descriptors = builder.AutoInitializer.LoadFromAttribute(builder.AutowireBaseType, builder.DependencyContext);

            if (initBeforeAction != null)
                foreach (var item in descriptors)
                    initBeforeAction(item.Value);

            if (initAction != null)
                builder.AutoInitializer.Init(descriptors, initAction);
            return builder.KernelService;
        }

        /// <summary>
        /// 执行自动装配构建
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="initAction"></param>
        /// <returns></returns>
        public static IKernelConfigure Build(this IAutowireConfigureBuilder builder, Action<AutowireDescriptor> initBeforeAction, Action<AutowireDescriptor> initAction)
        {
            IDictionary<TypeInfo, AutowireDescriptor> descriptors;
            if (!builder.AutowireBaseTypeIsAttribute)
                descriptors = builder.AutoInitializer.LoadFrom(builder.AutowireBaseType, builder.Replaces, builder.Removes, builder.DependencyContext);
            else
                descriptors = builder.AutoInitializer.LoadFromAttribute(builder.AutowireBaseType, builder.DependencyContext);

            if (initBeforeAction != null)
                foreach (var item in descriptors)
                    initBeforeAction(item.Value);

            if (initAction != null)
                builder.AutoInitializer.Init(descriptors, initAction);
            return builder.KernelConfigure;
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <typeparam name="TMatchType"></typeparam>
        /// <typeparam name="TReplceType"></typeparam>
        /// <param name="automaticBuilder"></param>
        /// <returns></returns>
        public static T Replace<TMatchType, TReplceType, T>(this T automaticBuilder)
            where T : IAutowireBuilder
        {
            var newType = typeof(TReplceType).GetTypeInfo();
            if (!newType.IsSubTypeOf(automaticBuilder.AutowireBaseType))
                throw new ArgumentException($"replace type is not sub type from: {automaticBuilder.AutowireBaseType}");
            automaticBuilder.Replaces[typeof(TMatchType).GetTypeInfo()] = typeof(TReplceType).GetTypeInfo();
            return automaticBuilder;
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
