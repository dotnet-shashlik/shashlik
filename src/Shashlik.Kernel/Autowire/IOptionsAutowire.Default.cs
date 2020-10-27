using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPOI.Util.Collections;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Autowire
{
    public class DefaultOptionsAutowire : IOptionsAutowire
    {
        /// <summary>
        /// 需要自动装配的类型
        /// </summary>
        public HashSet<Type> Disabled { get; } = new HashSet<Type>();

        public IKernelServices ConfigureAll(IKernelServices kernelServices)
        {
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new[] {typeof(IServiceCollection), typeof(IConfiguration)});
            if (method == null)
                throw new InvalidOperationException(
                    $"Cannot find method: OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(this IServiceCollection services, IConfiguration config).");

            var services = kernelServices.Services;
            services.AddOptions();

            var dic =
                ReflectHelper.GetTypesAndAttribute<AutoOptionsAttribute>(kernelServices.ScanFromDependencyContext);

            foreach (var (key, value) in dic)
            {
                if (Disabled.Contains(key))
                    continue;

                var sectionName = value.Section;
                var section = kernelServices.RootConfiguration.GetSection(sectionName);

                method.MakeGenericMethod(key)
                    .Invoke(null, new object[]
                    {
                        services, section
                    });
            }

            return kernelServices;
        }
    }
}