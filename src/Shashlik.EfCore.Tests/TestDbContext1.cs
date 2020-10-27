using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    public class TestDbContext1 : ShashlikDbContext<TestDbContext1>
    {
        public TestDbContext1(DbContextOptions<TestDbContext1> options) : base(options)
        {
        }
    }
    
    // public class BloggingContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    // {
    //     public TestDbContext CreateDbContext(string[] args)
    //     {
    //         var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
    //         var conn = "server=192.168.50.178;database=efcore_test;user=root;password=jizhen.cool.0416;Pooling=True;Min Pool Size=3;Max Pool Size=5;";
    //         optionsBuilder.UseMySql(conn, db => { db.MigrationsAssembly(typeof(EfCoreTestAutowire).Assembly.FullName); });
    //         return new TestDbContext(optionsBuilder.Options);
    //     }
    // }
}