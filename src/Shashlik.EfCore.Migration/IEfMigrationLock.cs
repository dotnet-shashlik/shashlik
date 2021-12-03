using System;

namespace Shashlik.EfCore.Migration
{
    /// <summary>
    /// EF数据库迁移锁,需要注册为单例M<para></para>
    /// 如果应用是集群式部署
    /// </summary>
    public interface IEfMigrationLock
    {
        IDisposable Lock();
    }
}