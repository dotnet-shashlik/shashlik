using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Ids4.PostgreSqlStore
{
    /// <summary>
    /// ids4 postgresql数据库存储配置
    /// </summary>
    public class Ids4PostgreSqlStoreAutowire : IIds4ExtensionAutowire
    {
        public Ids4PostgreSqlStoreAutowire(IOptions<Ids4PostgreSqlStoreOptions> options,
            IKernelServices kernelServices)
        {
            Options = options.Value;
            KernelServices = kernelServices;
        }

        private Ids4PostgreSqlStoreOptions Options { get; }

        private IKernelServices KernelServices { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            var conn = Options.ConnectionString;
            if (Options.EnableConfigurationStore || Options.EnableOperationalStore)
            {
                if (conn.IsNullOrWhiteSpace())
                {
                    conn = KernelServices.RootConfiguration.GetConnectionString("Default");
                    KernelServices.Services.Configure<Ids4PostgreSqlStoreOptions>(r => { r.ConnectionString = conn; });
                }

                if (conn.IsNullOrWhiteSpace())
                    throw new InvalidOperationException($"ConnectionString can not be empty.");
            }


            if (Options.EnableConfigurationStore)
            {
                builder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseNpgsql(conn!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4PostgreSqlStoreAutowire).Assembly.GetName()
                                    .FullName);
                            });
                    };
                });
            }


            if (Options.EnableOperationalStore)
            {
                builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseNpgsql(conn!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4PostgreSqlStoreAutowire).Assembly.GetName()
                                    .FullName);
                            });
                    };
                    // 每小时清除已过期的token
                    options.EnableTokenCleanup = true;
                });
            }
        }
    }
}