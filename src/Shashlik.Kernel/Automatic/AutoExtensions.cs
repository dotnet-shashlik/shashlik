using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using Shashlik.Kernel.Automatic.Attributes;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shashlik.Kernel.Automatic
{
    public static class AutoExtensions
    {
        /// <summary>
        /// 自动装配服务
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        public static void AddAutomaticConfigureService(
            this IKernelBuilder kernelBuilder,
            IConfiguration rootConfiguration,
            IDictionary<TypeInfo, TypeInfo> replaces = null,
            DependencyContext dependencyContext = null)
        {
            kernelBuilder.Services.TryAddSingleton<IAutoInitializer>(new DefaultAutoInitializer());
            using var serviceProvider = kernelBuilder.Services.BuildServiceProvider();
            var autoInitializer = serviceProvider.GetRequiredService<IAutoInitializer>();

            var autoOptions = autoInitializer.ScanAttribute<AutoOptionsAttribute>();
            LoadAutoOptions(autoOptions, kernelBuilder.Services, rootConfiguration);

            var descriptors = autoInitializer.Scan<IAutoServices>(replaces);

            foreach (var item in descriptors)
                kernelBuilder.Services.TryAddSingleton(item.Key, item.Key);

            autoInitializer.Init(descriptors, r =>
            {
                using var serviceProvider = kernelBuilder.Services.BuildServiceProvider();
                var serviceInstance = serviceProvider.GetService(r.ServiceType);
                var instance = serviceInstance as IAutoServices;
                if (instance != null)
                    // IAutoServices类型
                    instance.ConfigureServices(kernelBuilder, rootConfiguration);
                else
                {
                    // 非IAutoServices类型,约定方法签名是ConfigureServices(IKernelBuilder kernelBuilder, IConfiguration configuration)
                    var method = r.ServiceType.GetMethods(BindingFlags.Instance)
                                  .FirstOrDefault(r =>
                                        r.Name == nameof(instance.ConfigureServices)
                                        && !r.IsGenericMethod
                                        && r.GetParameters().Length == 2
                                        && r.GetParameters()[0].ParameterType == typeof(IKernelBuilder)
                                        && r.GetParameters()[1].ParameterType == typeof(IConfiguration));
                    if (method == null)
                        throw new System.Exception($"replace auto service\"{r}\" error: can't find method definetion \"{nameof(instance.ConfigureServices)}(IKernelBuilder kernelBuilder, IConfiguration rootConfiguration)\".");

                    method.Invoke(serviceInstance, new object[] { kernelBuilder, rootConfiguration });
                }
            });
        }

        static void LoadAutoOptions(IDictionary<TypeInfo, AutoOptionsAttribute> types, IServiceCollection services, IConfiguration rootConfiguration)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new Type[] { typeof(IServiceCollection), typeof(IConfiguration) });

            foreach (var item in types)
            {
                if (item.Key.IsAbstract)
                    continue;
                method.MakeGenericMethod(item.Key).Invoke(null, new object[] { services, rootConfiguration.GetSection(item.Value.Section) });
            }
        }

        /// <summary>
        /// 自动配置
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="replaces"></param>
        /// <param name="dependencyContext"></param>
        public static void UseAutomaticConfigure(
            this IKernelConfigure kernelConfigure,
            IDictionary<TypeInfo, TypeInfo> replaces = null,
            DependencyContext dependencyContext = null)
        {
            var autoInitializer = kernelConfigure.ServiceProvider.GetRequiredService<IAutoInitializer>();
            var autoConfigs = autoInitializer.Scan<IAutoConfigure>(replaces, dependencyContext);

            autoInitializer.Init(autoConfigs, r =>
            {
                (r.ServiceInstance as IAutoConfigure).Configure(kernelConfigure);
            });
        }
    }
}
