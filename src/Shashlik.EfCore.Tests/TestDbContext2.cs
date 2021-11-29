using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shashlik.EfCore.Migration;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj

    [AutoMigration]
    public class TestDbContext2 : ShashlikDbContext<TestDbContext2>
    {
        public TestDbContext2(DbContextOptions<TestDbContext2> options) : base(options)
        {
        }
    }
}