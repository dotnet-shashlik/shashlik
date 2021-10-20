using System;

namespace Shashlik.EfCore
{
    /// <summary>
    /// EF数据库迁移锁
    /// </summary>
    public interface IEfMigrationLock
    {
        IDisposable Lock(string key);
    }
}