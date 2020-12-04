using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.Int32.MySql
{
    public class IdentityMySqlAutowire : IServiceAutowire
    {
        public IdentityMySqlAutowire(
            IOptions<ShashlikIdentityOptions> options,
            IOptions<ShashlikIdentityMySqlOptions> mysqlOptions)
        {
            Options = options.Value;
            MySqlOptions = mysqlOptions.Value;
        }

        private ShashlikIdentityOptions Options { get; }
        private ShashlikIdentityMySqlOptions MySqlOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            var conn = MySqlOptions.ConnectionString;
            if (string.IsNullOrWhiteSpace(conn))
            {
                conn = kernelService.RootConfiguration.GetConnectionString("Default");
                kernelService.Services.Configure<ShashlikIdentityMySqlOptions>(r => { r.ConnectionString = conn; });
            }

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException($"ConnectionString can not be empty");

            kernelService.Services.AddDbContextPool<ShashlikIdentityDbContext>(options =>
            {
                options.UseMySql(
                    conn,
                    ServerVersion.FromString(MySqlOptions.DbVersion),
                    db =>
                    {
                        db.MigrationsAssembly(MySqlOptions.MigrationAssembly.EmptyToNull() ?? typeof(IdentityMySqlAutowire).Assembly.FullName);
                    });
            }, MySqlOptions.DbContextPoolSize);

            if (MySqlOptions.AutoMigration)
                kernelService.Services.AddAutoMigration<ShashlikIdentityDbContext>();
        }
    }
}