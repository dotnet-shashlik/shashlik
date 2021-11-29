using System;

namespace Shashlik.EfCore.Migration
{
    /// <summary>
    /// EF数据库迁移锁
    /// </summary>
    public interface IEfMigrationLock
    {
        IDisposable Lock();
    }
}