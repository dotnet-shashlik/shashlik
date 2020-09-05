using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;

namespace Shashlik.Ids4.PostgreSqlStore
{
    /// <summary>
    /// ids4 postgresql数据库存储配置
    /// </summary>
    public class Ids4PostgreSqlStoreConfigure : IIds4ConfigureServices
    {
        public Ids4PostgreSqlStoreConfigure(IOptions<IdsdPostgreSqlStoreOptions> options,
            IKernelServices kernelServices)
        {
            Options = options.Value;
            KernelServices = kernelServices;
        }

        private IdsdPostgreSqlStoreOptions Options { get; }

        private IKernelServices KernelServices { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (Options.EnableClientStore)
                builder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseNpgsql(Options.ConnectionString!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(IdsdPostgreSqlStoreOptions).Assembly.GetName().FullName);
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
                                mig.MigrationsAssembly(typeof(IdsdPostgreSqlStoreOptions).Assembly.GetName().FullName);
                            });
                    };
                });

            // 执行client store 数据库迁移
            if (Options.AutoMigration && Options.EnableClientStore)
                KernelServices.Services.Migration<ConfigurationDbContext>();

            // 执行operation store 数据库迁移
            if (Options.AutoMigration && Options.EnableOperationalStore)
                KernelServices.Services.Migration<PersistedGrantDbContext>();
        }
    }
}