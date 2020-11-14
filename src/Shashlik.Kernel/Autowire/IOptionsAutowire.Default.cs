using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Autowire
{
    public class DefaultOptionsAutowire : IOptionsAutowire
    {
        public void ConfigureAll(IKernelServices kernelServices, IEnumerable<Type> disabledAutoOptionTypes)
        {
            if (kernelServices is null) throw new ArgumentNullException(nameof(kernelServices));
            if (disabledAutoOptionTypes is null) throw new ArgumentNullException(nameof(disabledAutoOptionTypes));

            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new[] {typeof(IServiceCollection), typeof(IConfiguration)});
            if (method is null)
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

            using var serviceProvider = kernelServices.Services.BuildServiceProvider();

            // options model validation
            foreach (var (key, value) in dic)
            {
                var optionsTypes = typeof(IOptions<>).MakeGenericType(key);
                // ReSharper disable once PossibleMultipleEnumeration
                if (disabledAutoOptionTypes.Contains(key))
                    continue;

                var optionValue = serviceProvider.GetService(optionsTypes).GetPropertyValue("Value");

                var res = ValidationHelper.Validate(optionValue, serviceProvider);
                if (res.Count > 0)
                    throw new OptionsValidationException(value.Section, key, res.Select(r => r.ErrorMessage));
            }
        }
    }
}