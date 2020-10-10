﻿using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.DataProtector.PostgreSql
{
    /// <summary>
    /// 使用CSRedisCore共享DataProtector的密钥存储,解决集群缓存密钥存储问题
    /// </summary>
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


            if (Options.Key.IsNullOrWhiteSpace())
                throw new InvalidOperationException($"Certificate must be contains private key.");


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