using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    public class TestDbContext4 : ShashlikDbContext<TestDbContext4>
    {
        public TestDbContext4(DbContextOptions<TestDbContext4> options) : base(options)
        {
        }
    }
}