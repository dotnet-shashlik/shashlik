using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.EfCore.Tests.Entities
{
    public class Roles : IEntity<int>
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public Users User { get; set; }

        public string Name { get; set; }
    }

    public class RolesConfig : IEntityTypeConfiguration<Roles>
    {
        public void Configure(EntityTypeBuilder<Roles> builder)
        {
            builder.Property(r => r.Name).HasMaxLength(255).IsRequired();
            builder.HasOne(r => r.User).WithMany(r => r.Roles).HasForeignKey(r => r.UserId);
        }
    }
}