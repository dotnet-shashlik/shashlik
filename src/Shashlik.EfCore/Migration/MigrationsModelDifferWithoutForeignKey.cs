using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Shashlik.EfCore.Migration;

/// <summary>
/// efcore创建迁移时移除外键的创建
/// </summary>
[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
public class MigrationsModelDifferWithoutForeignKey : MigrationsModelDiffer
{
    public MigrationsModelDifferWithoutForeignKey(IRelationalTypeMappingSource typeMappingSource,
        IMigrationsAnnotationProvider migrationsAnnotations, IChangeDetector changeDetector,
        IUpdateAdapterFactory updateAdapterFactory, CommandBatchPreparerDependencies commandBatchPreparerDependencies) :
        base(typeMappingSource, migrationsAnnotations, changeDetector, updateAdapterFactory,
            commandBatchPreparerDependencies)
    {
    }

    public override IReadOnlyList<MigrationOperation> GetDifferences(IRelationalModel? source, IRelationalModel? target)
    {
        var operations = base.GetDifferences(source, target)
            .Where(op => op is not AddForeignKeyOperation)
            .Where(op => op is not DropForeignKeyOperation)
            .ToList();

        foreach (var operation in operations.OfType<CreateTableOperation>())
            operation.ForeignKeys?.Clear();

        return operations;
    }
}