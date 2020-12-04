using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    public class TestDbContext5 : ShashlikDbContext<TestDbContext5>
    {
        public TestDbContext5(DbContextOptions<TestDbContext5> options) : base(options)
        {
        }
    }
}