using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shashlik.Identity.Entities;

namespace Shashlik.Identity
{
    public class ShashlikIdentityDbContext : IdentityDbContext<Users, Roles, int>
    {
        public ShashlikIdentityDbContext(DbContextOptions<ShashlikIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(this.GetService<UsersConfig>());
        }
    }
}