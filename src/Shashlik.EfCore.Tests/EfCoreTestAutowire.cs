using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Tests
{
    [Transient]
    public class EfCoreTestAutowire : IServiceAssembler
    {
        public void Configure(IKernelServices kernelServices)
        {
            kernelServices.Services.AddDbContext<TestDbContext1>(r =>
                {
                    var conn = kernelServices.RootConfiguration.GetValue<string>("ConnectionStrings:Conn1");
                    r.UseMySql(conn, ServerVersion.AutoDetect(conn),
                        db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
                })
                ;
        }
    }
}