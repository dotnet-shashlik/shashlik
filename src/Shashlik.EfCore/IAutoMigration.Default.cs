using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore
{
    public sealed class AutoMigration<TDbContext> : IAutoMigration where TDbContext : DbContext
    {
    }
}