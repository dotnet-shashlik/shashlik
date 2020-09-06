using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;

namespace Shashlik.Identity.MySql
{
    public class IdentityMySqlConfigure : IAutowiredConfigureServices
    {
        public IdentityMySqlConfigure(IOptions<ShashlikIdentityOptions> options)
        {
            Options = options.Value;
        }

        private ShashlikIdentityOptions Options { get; }

        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.Services.AddDbContext<ShashlikIdentityDbContext>(options =>
            {
                options.UseMySql(Options.ConnectionString!,
                    db => { db.MigrationsAssembly(Options.MigrationAssembly ?? this.GetType().Assembly.FullName); });
            });

            if (Options.AutoMigration)
                kernelService.Services.Migration<ShashlikDbContext>();
        }
    }
}