using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Exceptions;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Options
{
    public class DefaultOptionsAssembler : IOptionsAssembler
    {
        public void ConfigureAll(IKernelServices kernelServices)
        {
            if (kernelServices is null) throw new ArgumentNullException(nameof(kernelServices));

            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(string), typeof(IConfiguration) });
            if (method is null)
                throw new MissingMethodException(
                    "OptionsConfigurationServiceCollectionExtensions",
                    "Configure<TOptions>(this IServiceCollection services, string name, IConfiguration config)");

            var services = kernelServices.Services;

            var dic =
                ReflectionHelper.GetTypesAndAttribute<AutoOptionsAttribute>(kernelServices.ScanFromDependencyContext);

            foreach (var (key, value) in dic)
            {
                var sectionName = value.Section;
                var section = kernelServices.RootConfiguration.GetSection(sectionName);

                method.MakeGenericMethod(key)
                    .Invoke(null, new object[]
                    {
                        services, value.Name, section
                    });
            }

            using var serviceProvider = kernelServices.Services.BuildServiceProvider();

            // options model validation
            foreach (var (key, value) in dic)
            {
                var optionsTypes = typeof(IOptions<>).MakeGenericType(key);

                var (_, obj) = serviceProvider.GetRequiredService(optionsTypes).GetPropertyValue("Value");

                if (!obj!.TryValidateObjectRecursion(out var results))
                    throw new KernelAssembleException(
                        $"Invalid option, section: {value.Section}, option type: {key}, errors:{Environment.NewLine}{results.Select(r => r.ErrorMessage).Join(Environment.NewLine)}");
            }
        }
    }
}