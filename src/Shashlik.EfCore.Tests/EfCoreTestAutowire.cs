using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

namespace Shashlik.EfCore.Tests
{
    public class EfCoreTestAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddDbContext<TestDbContext>(r =>
            {
                r.UseMySql(kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Test"),
                    db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
            });
        }
    }
}