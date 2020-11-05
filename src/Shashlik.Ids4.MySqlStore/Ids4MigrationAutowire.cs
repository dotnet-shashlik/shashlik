using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Ids4.MySqlStore
{
    [Order(110)]
    public class Ids4MigrationAutowire : IApplicationStartAutowire
    {
        public Ids4MigrationAutowire(IServiceProvider serviceProvider, IOptions<Ids4MySqlStoreOptions> options,
            IOptions<Ids4Options> ids4Options)
        {
            ServiceProvider = serviceProvider;
            Options = options;
            Ids4Options = ids4Options;
        }

        private IServiceProvider ServiceProvider { get; }
        private IOptions<Ids4MySqlStoreOptions> Options { get; }
        private IOptions<Ids4Options> Ids4Options { get; }

        public async Task OnStart(CancellationToken cancellationToken)
        {
            if (!Ids4Options.Value.Enable || !Options.Value.AutoMigration)
                return;

            if (Options.Value.EnableConfigurationStore)
            {
                await ServiceProvider.MigrationAsync<ConfigurationDbContext>();
            }

            if (Options.Value.EnableOperationalStore)
            {
                await ServiceProvider.MigrationAsync<PersistedGrantDbContext>();
            }
        }
    }
}