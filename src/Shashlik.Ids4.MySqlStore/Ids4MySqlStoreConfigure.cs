using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Ids4.MySqlStore
{
    /// <summary>
    /// ids4 mysql 数据库存储配置
    /// </summary>
    public class Ids4MySqlStoreConfigure : IIdentityServerBuilderConfigure, IAutowiredConfigure
    {
        public Ids4MySqlStoreConfigure(IOptions<Ids4MySqlStoreOptions> options, IKernelServices kernelServices)
        {
            Options = options.Value;
            KernelServices = kernelServices;
        }

        private Ids4MySqlStoreOptions Options { get; }

        private IKernelServices KernelServices { get; }

        public void ConfigureIds4(IIdentityServerBuilder builder)
        {
            if (Options.EnableConfigurationStore)
                builder.AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseMySql(Options.ConnectionString!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4MySqlStoreConfigure).Assembly.GetName().FullName);
                            });
                    };
                });


            if (Options.EnableOperationalStore)
                builder.AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = dbOptions =>
                    {
                        dbOptions.UseMySql(Options.ConnectionString!,
                            mig =>
                            {
                                mig.MigrationsAssembly(typeof(Ids4MySqlStoreConfigure).Assembly.GetName().FullName);
                            });
                    };
                });
        }

        public void Configure(IKernelConfigure kernelConfigure)
        {
            // 执行client store 数据库迁移
            if (Options.AutoMigration && Options.EnableConfigurationStore)
                kernelConfigure.ServiceProvider.Migration<ConfigurationDbContext>();

            // 执行operation store 数据库迁移
            if (Options.AutoMigration && Options.EnableOperationalStore)
                kernelConfigure.ServiceProvider.Migration<PersistedGrantDbContext>();
        }
    }
}