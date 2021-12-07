using System;

namespace Shashlik.EfCore.Migration
{
    /// <summary>
    /// EF数据库迁移锁,需要注册为单例<para></para>
    /// 如果应用是集群式部署,一般都需要加锁执行迁移
    /// </summary>
    public interface IEfMigrationLock
    {
        IDisposable Lock();
    }
}