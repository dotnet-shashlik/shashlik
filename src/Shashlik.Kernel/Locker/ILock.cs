using System;

namespace Shashlik.Kernel.Locker
{
    /// <summary>
    /// 锁
    /// </summary>
    public interface ILock
    {
        /// <summary>
        /// 开始锁定
        /// </summary>
        /// <param name="key">锁key</param>
        /// <param name="lockSecond">锁定时长,秒</param>
        /// <param name="autoDelay">是否自动延期</param>
        /// <param name="waitTimeout">等待锁的超时时间,秒</param>
        /// <returns>锁释放实例</returns>
        IDisposable Lock(string key, int lockSecond, bool autoDelay = true, int waitTimeout = 60);
    }
}