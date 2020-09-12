﻿using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Ids4.PostgreSqlStore
{
    //TODO: 生成迁移
    /// <summary>
    /// ids4 postgresql数据库存储配置
    /// </summary>
    public class Ids4PostgreSqlStoreConfigure : IIdentityServerBuilderConfigure
    {
        public Ids4PostgreSqlStoreConfigure(IOptions<Ids4PostgreSqlStoreOptions> options,
            IKernelServices kernelServices)
        {
            Options = options.Value;
            KernelServices = kernelServices;
        }

        private Ids4PostgreSqlStoreOptions Options { get; }

        private IKernelServices KernelServices { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (Options.EnableConfigurationStore)
                builder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseNpgsql(Options.ConnectionString!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4PostgreSqlStoreOptions).Assembly.GetName().FullName);
                            });
                    };
                });

            if (Options.EnableOperationalStore)
                builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseNpgsql(Options.ConnectionString!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4PostgreSqlStoreOptions).Assembly.GetName().FullName);
                            });
                    };
                });

            // 执行client store 数据库迁移
            if (Options.AutoMigration && Options.EnableConfigurationStore)
                KernelServices.Services.Migration<ConfigurationDbContext>();

            // 执行operation store 数据库迁移
            if (Options.AutoMigration && Options.EnableOperationalStore)
                KernelServices.Services.Migration<PersistedGrantDbContext>();
        }
    }
}