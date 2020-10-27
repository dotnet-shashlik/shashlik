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
            kernelServices.Services.AddDbContext<TestDbContext1>(r =>
                {
                    var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Default");
                    r.UseMySql(conn, db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
                })
                .Migration<TestDbContext1>();
        }
    }
}