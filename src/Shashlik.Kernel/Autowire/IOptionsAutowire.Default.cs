using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Autowire
{
    public class DefaultOptionsAutowire : IOptionsAutowire
    {
        public void ConfigureAll(IKernelServices kernelServices, IEnumerable<Type> disabledAutoOptionTypes)
        {
            if (kernelServices == null) throw new ArgumentNullException(nameof(kernelServices));
            if (disabledAutoOptionTypes == null) throw new ArgumentNullException(nameof(disabledAutoOptionTypes));

            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new[] {typeof(IServiceCollection), typeof(IConfiguration)});
            if (method == null)
                throw new MethodAccessException(
                    $"Cannot find method: OptionsConfigurationServiceCollectionExtensions.Configure<TOptions>(this IServiceCollection services, IConfiguration config).");

            var services = kernelServices.Services;
            services.AddOptions();

            var dic =
                ReflectHelper.GetTypesAndAttribute<AutoOptionsAttribute>(kernelServices.ScanFromDependencyContext);

            foreach (var (key, value) in dic)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (disabledAutoOptionTypes.Contains(key))
                    continue;

                var sectionName = value.Section;
                var section = kernelServices.RootConfiguration.GetSection(sectionName);

                method.MakeGenericMethod(key)
                    .Invoke(null, new object[]
                    {
                        services, section
                    });
            }
        }
    }
}