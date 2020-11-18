using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Helpers;

namespace Shashlik.EfCore
{
    /// <summary>
    /// DbContext自动迁移装配,IApplicationStartAutowire装配顺序100
    /// </summary>
    [Order(100)]
    public class AutoMigrationAutowire : IApplicationStartAutowire
    {
        public AutoMigrationAutowire(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
        }

        private IServiceProvider ServiceProvider { get; }
        private IConfiguration Configuration { get; }

        public async Task OnStart(CancellationToken cancellationToken)
        {
            var dbContexts = ReflectionHelper.GetFinalSubTypes<DbContext>();
            foreach (var dbContext in dbContexts)
            {
                if (GetEnableAutoMigration(dbContext))
                    await ServiceProvider.MigrationAsync(dbContext);
            }
        }

        private bool GetEnableAutoMigration(Type type)
        {
            var autoMigrationAttribute = type.GetCustomAttribute<AutoMigrationAttribute>();
            return autoMigrationAttribute != null && autoMigrationAttribute.GetEnableAutoMigration(Configuration);
        }
    }
}