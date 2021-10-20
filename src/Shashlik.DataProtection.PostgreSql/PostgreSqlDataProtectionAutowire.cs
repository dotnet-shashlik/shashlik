using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    [Transient]
    public class PostgreSqlDataProtectionAutowire : IServiceAssembler
    {
        public PostgreSqlDataProtectionAutowire(IOptions<PostgreSqlDataProtectionOptions> options)
        {
            Options = options.Value;
        }

        private PostgreSqlDataProtectionOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;
            if (Options.ConnectionString.IsNullOrWhiteSpace())
            {
                Options.ConnectionString = kernelService.RootConfiguration.GetConnectionString("Default");
                kernelService.Services.Configure<PostgreSqlDataProtectionOptions>(r => { r.ConnectionString = Options.ConnectionString; });
            }

            if (Options.ConnectionString.IsNullOrWhiteSpace())
                throw new OptionsValidationException("Shashlik.DataProtection.PostgreSql.ConnectionString",
                    typeof(PostgreSqlDataProtectionOptions),
                    new[] {"PostgreSql data protection store database connection string can not be empty."});

            kernelService.Services.AddDataProtection()
                // 设置应用名称
                .SetApplicationName(Options.ApplicationName)
                ;

            kernelService.Services.Configure<KeyManagementOptions>(options => { options.XmlRepository = new PostgreSqlXmlRepository(Options); });
        }
    }
}