using System;

namespace Shashlik.EfCore.Migration
{
    /// <summary>
    /// EF数据库迁移锁,需要注册为单例
    /// </summary>
    public interface IEfMigrationLock
    {
        IDisposable Lock();
    }
}