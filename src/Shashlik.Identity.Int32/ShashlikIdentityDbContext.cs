using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Shashlik.EfCore;
using Shashlik.Identity.Options;

// ReSharper disable CheckNamespace

namespace Shashlik.Identity
{
    [AutoMigration(typeof(bool), "Shashlik.Identity.AutoMigration", true)]
    public class ShashlikIdentityDbContext : IdentityDbContext<Users, Roles, int>
    {
        public ShashlikIdentityDbContext(DbContextOptions<ShashlikIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            IdentityUserExtendsOptions options = this.GetService<IOptions<IdentityUserExtendsOptions>>().Value;

            modelBuilder.Entity<Users>().Property(r => r.IdCard).HasMaxLength(32).IsRequired(options.RequireIdCard);
            modelBuilder.Entity<Users>().Property(r => r.RealName).HasMaxLength(32).IsRequired(options.RequireRealName);
            modelBuilder.Entity<Users>().Property(r => r.NickName).HasMaxLength(255)
                .IsRequired(options.RequireNickName);
            modelBuilder.Entity<Users>().Property(r => r.Avatar).HasMaxLength(255).IsRequired(options.RequireAvatar);
            modelBuilder.Entity<Users>().Property(r => r.Birthday).IsRequired(options.RequireBirthday);
            modelBuilder.Entity<Users>().HasIndex(r => r.IdCard).IsUnique(options.RequireUniqueIdCard);
            modelBuilder.Entity<Users>().HasIndex(r => r.PhoneNumber).IsUnique(options.RequireUniquePhoneNumber);
        }
    }
}