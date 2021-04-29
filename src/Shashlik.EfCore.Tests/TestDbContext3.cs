using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    [AutoMigration]
    public class TestDbContext3 : ShashlikDbContext<TestDbContext3>
    {
        public TestDbContext3(DbContextOptions<TestDbContext3> options) : base(options)
        {
        }
    }
}