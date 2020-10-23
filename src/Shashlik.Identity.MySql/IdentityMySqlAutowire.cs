using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Identity.Options;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Extensions;

namespace Shashlik.Identity.MySql
{
    public class IdentityMySqlAutowire : IServiceAutowire
    {
        public IdentityMySqlAutowire(IOptions<ShashlikIdentityOptions> options)
        {
            Options = options.Value;
        }

        private ShashlikIdentityOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            var conn = Options.ConnectionString;
            if (conn.IsNullOrWhiteSpace())
            {
                conn = kernelService.RootConfiguration.GetConnectionString("Default");
                kernelService.Services.Configure<ShashlikIdentityOptions>(r => { r.ConnectionString = conn; });
            }

            if (conn.IsNullOrWhiteSpace())
                throw new InvalidOperationException($"ConnectionString can not be empty.");

            kernelService.Services.AddDbContext<ShashlikIdentityDbContext>(options =>
            {
                options.UseMySql(conn!,
                    db =>
                    {
                        db.MigrationsAssembly(Options.MigrationAssembly.EmptyToNull() ??
                                              typeof(IdentityMySqlAutowire).Assembly.FullName);
                    });
            });

            if (Options.AutoMigration)
                kernelService.Services.Migration<ShashlikIdentityDbContext>();
        }
    }
}