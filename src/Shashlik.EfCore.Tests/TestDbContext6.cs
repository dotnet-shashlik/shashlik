using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    public class TestDbContext6 : ShashlikDbContext<TestDbContext6>
    {
        public TestDbContext6(DbContextOptions<TestDbContext6> options) : base(options)
        {
        }
    }
}