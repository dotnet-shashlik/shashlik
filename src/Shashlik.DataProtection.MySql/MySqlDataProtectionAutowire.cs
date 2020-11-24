using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    public class MySqlDataProtectionAutowire : IServiceAutowire
    {
        public MySqlDataProtectionAutowire(IOptions<MySqlDataProtectionOptions> options)
        {
            Options = options.Value;
        }

        private MySqlDataProtectionOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;
            if (Options.ConnectionString.IsNullOrWhiteSpace())
            {
                Options.ConnectionString = kernelService.RootConfiguration.GetConnectionString("Default");
                kernelService.Services.Configure<MySqlDataProtectionOptions>(r =>
                {
                    r.ConnectionString = Options.ConnectionString;
                });
            }

            if (Options.ConnectionString.IsNullOrWhiteSpace())
                throw new OptionsValidationException("Shashlik.DataProtection.MySql.ConnectionString",
                    typeof(MySqlDataProtectionOptions),
                    new[] {"MySql data protection store database connection string can not be empty."});

            kernelService.Services.AddDataProtection()
                // 设置应用名称
                .SetApplicationName(Options.ApplicationName);

            kernelService.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new MySqlXmlRepository(Options);
            });
        }
    }
}