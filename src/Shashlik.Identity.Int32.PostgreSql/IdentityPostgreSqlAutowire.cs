using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.Int32.PostgreSql
{
    public class IdentityPostgreSqlAutowire : IServiceAutowire
    {
        public IdentityPostgreSqlAutowire(
            IOptions<ShashlikIdentityOptions> options,
            IOptions<ShashlikIdentityPostgreSqlOptions> postgreSqlOptions)
        {
            Options = options.Value;
            PostgreSqlOptions = postgreSqlOptions.Value;
        }

        private ShashlikIdentityOptions Options { get; }
        private ShashlikIdentityPostgreSqlOptions PostgreSqlOptions { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            var conn = PostgreSqlOptions.ConnectionString;
            if (string.IsNullOrWhiteSpace(conn))
            {
                conn = kernelService.RootConfiguration.GetConnectionString("Default");
                kernelService.Services.Configure<ShashlikIdentityPostgreSqlOptions>(r => { r.ConnectionString = conn; });
            }

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException($"ConnectionString can not be empty");

            kernelService.Services.AddDbContext<ShashlikIdentityDbContext>(options =>
            {
                options.UseNpgsql(conn!,
                    db =>
                    {
                        db.MigrationsAssembly(PostgreSqlOptions.MigrationAssembly.EmptyToNull() ??
                                              typeof(IdentityPostgreSqlAutowire).Assembly.FullName);
                    });
            });
        }
    }
}