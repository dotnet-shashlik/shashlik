using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Identity.PostgreSql
{
    public class IdentityPostgreSqlConfigure : IAutowiredConfigureServices
    {
        public IdentityPostgreSqlConfigure(IOptions<ShashlikIdentityOptions> options)
        {
            Options = options.Value;
        }

        private ShashlikIdentityOptions Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.Services.AddDbContext<ShashlikIdentityDbContext>(options =>
            {
                options.UseNpgsql(Options.ConnectionString!,
                    db => { db.MigrationsAssembly(Options.MigrationAssembly ?? this.GetType().Assembly.FullName); });
            });

            if (Options.AutoMigration)
                kernelService.Services.Migration<ShashlikDbContext>();
        }
    }
}