using Microsoft.EntityFrameworkCore;
using Shashlik.EfCore.Transactional;

namespace Shashlik.EfCore.Tests
{
    // dotnet ef migrations add init -c TestDbContext  -o Migrations -p ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj -s ./Shashlik.EfCore.Tests/Shashlik.EfCore.Tests.csproj
    
    [DefaultTransactional]
    public class TestDbContext : ShashlikDbContext<TestDbContext>
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
    }
}