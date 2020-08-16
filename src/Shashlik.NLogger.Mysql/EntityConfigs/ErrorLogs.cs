using Shashlik.NLogger.Loggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.NLogger.Mysql.EntityConfigs
{
    public class ErrorLogsConfig : IEntityTypeConfiguration<ErrorLogs>
    {
        public void Configure(EntityTypeBuilder<ErrorLogs> builder)
        {
            builder.Property(r => r.User).HasMaxLength(255);
            builder.Property(r => r.ClientId).HasMaxLength(255);
            builder.Property(r => r.Logger).HasMaxLength(255);
            builder.Property(r => r.Level).HasMaxLength(255);
            builder.Property(r => r.ClientIp).HasMaxLength(255);
            builder.Property(r => r.Method).HasMaxLength(255);
        }
    }
}
