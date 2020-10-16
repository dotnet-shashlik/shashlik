using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.DataProtector.PostgreSql
{
    public class PostgreSqlDataProtectorConfigure : IAutowiredConfigureServices
    {
        public PostgreSqlDataProtectorConfigure(IOptions<PostgreSqlDataProtectorOptions> options)
        {
            Options = options.Value;
        }

        private PostgreSqlDataProtectorOptions Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            if (Options.ConnectionString.IsNullOrWhiteSpace())
                throw new InvalidOperationException($"ConnectionString can not be empty.");

            kernelService.Services.AddDataProtection()
                // 设置应用名称
                .SetApplicationName(Options.ApplicationName);

            kernelService.Services.Configure<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new PostgreSqlXmlRepository(Options);
            });
        }
    }
}