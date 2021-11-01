using System;

namespace Shashlik.Redis
{
    /// <summary>
    /// 锁失败异常
    /// </summary>
    public class LockFailureException : Exception
    {
        public string LockKey { get; }

        public LockFailureException(string lockKey) : base($"Lock failure of {lockKey}")
        {
            LockKey = lockKey;
        }
    }
}