using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.MySqlStore
{
    /// <summary>
    /// ids4 mysql 数据库存储配置
    /// </summary>
    public class Ids4MySqlStoreAutowire : IIds4ExtensionAutowire
    {
        public Ids4MySqlStoreAutowire(IOptions<Ids4MySqlStoreOptions> options, IKernelServices kernelServices)
        {
            Options = options.Value;
            KernelServices = kernelServices;
        }

        private Ids4MySqlStoreOptions Options { get; }

        private IKernelServices KernelServices { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            var conn = Options.ConnectionString;
            if (Options.EnableConfigurationStore || Options.EnableOperationalStore)
            {
                if (string.IsNullOrWhiteSpace(conn))
                {
                    conn = KernelServices.RootConfiguration.GetConnectionString("Default");
                    KernelServices.Services.Configure<Ids4MySqlStoreOptions>(r => { r.ConnectionString = conn; });
                }

                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException($"ConnectionString can not be empty");
            }

            if (Options.EnableConfigurationStore)
            {
                builder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseMySql(
                            conn!,
                            ServerVersion.FromString(Options.DbVersion),
                            mig => { mig.MigrationsAssembly(typeof(Ids4MySqlStoreAutowire).Assembly.GetName().FullName); });
                    };
                });
            }

            if (Options.EnableOperationalStore)
            {
                builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseMySql(
                            conn!,
                            ServerVersion.FromString(Options.DbVersion),
                            mig => { mig.MigrationsAssembly(typeof(Ids4MySqlStoreAutowire).Assembly.GetName().FullName); });
                    };
                    // 每小时清除已过期的token
                    options.EnableTokenCleanup = true;
                });
            }
        }
    }
}