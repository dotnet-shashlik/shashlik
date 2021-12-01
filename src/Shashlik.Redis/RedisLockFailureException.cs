using System;

namespace Shashlik.Redis
{
    /// <summary>
    /// 锁失败异常
    /// </summary>
    public class RedisLockFailureException : Exception
    {
        public string LockKey { get; }

        public RedisLockFailureException(string lockKey) : base($"Lock failed on `{lockKey}`")
        {
            LockKey = lockKey;
        }
    }
}