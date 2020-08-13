using Guc.NLogger.Loggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.NLogger.Postgres.EntityConfigs
{
    public class OperationLogsConfig : IEntityTypeConfiguration<OperationLogs>
    {
        public void Configure(EntityTypeBuilder<OperationLogs> builder)
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
