using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.EfCore.Filter;

namespace Shashlik.EfCore.Tests.Entities
{
    public class Users : IEntity<int>, ISoftDeleted<DateTime>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<Roles> Roles { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeleteTime { get; set; }
    }

    public class UserConfig : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
        }
    }
}