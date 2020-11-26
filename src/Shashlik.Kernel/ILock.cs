using System;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 抽象的分布式锁,kernel内部内存锁实现
    /// </summary>
    public interface ILock
    {
        /// <summary>
        /// 开始锁定
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="lockSeconds">锁定时长, 秒</param>
        /// <param name="autoDelay">是否自动延期</param>
        /// <param name="waitTimeoutSeconds">等待锁的超时时间, 秒</param>
        /// <returns>锁释放实例</returns>
        IDisposable Lock(string key, int lockSeconds, bool autoDelay = true, int waitTimeoutSeconds = 60);
    }
}