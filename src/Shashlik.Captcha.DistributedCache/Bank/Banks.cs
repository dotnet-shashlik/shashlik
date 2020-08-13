using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guc.Features
{
    /// <summary>
    /// 银行
    /// </summary>
    public class Banks : IFeatureEntitiy
    {
        /// <summary>
        /// 银行id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行logo
        /// </summary>
        public string Logo { get; set; }
    }

    public class BanksConfig : IEntityTypeConfiguration<Banks>
    {
        public void Configure(EntityTypeBuilder<Banks> builder)
        {
            builder.ToTable("Feature_Banks");
            builder.Property(r => r.Id).HasMaxLength(32).IsRequired();
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).HasMaxLength(128).IsRequired();
            builder.Property(r => r.Logo).HasMaxLength(512);
        }
    }
}
