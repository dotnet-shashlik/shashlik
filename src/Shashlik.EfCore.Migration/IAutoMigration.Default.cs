using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore.Migration
{
    public sealed class AutoMigration<TDbContext> : IAutoMigration where TDbContext : DbContext
    {
    }
}