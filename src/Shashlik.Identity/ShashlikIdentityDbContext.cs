using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shashlik.EfCore;
using Shashlik.Identity.Entities;
using Shashlik.Kernel;

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
            modelBuilder.ApplyConfiguration(new UsersConfig());
        }
    }
}