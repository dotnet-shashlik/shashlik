using System;
using System.Collections.Concurrent;
using System.Threading;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Kernel.Exceptions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Locker.Memory
{
    [ConditionDependsOnMissing(typeof(ILock))]
    public class MemoryLock : ILock
    {
        private static readonly ConcurrentDictionary<string, AsyncLock> Lockers =
            new ConcurrentDictionary<string, AsyncLock>();

        public IDisposable Lock(string key, int lockSecond, bool autoDelay = true, int waitTimeout = 60)
        {
            key = $"ShashlikMemoryLock:{key}";
            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(waitTimeout));
            var asyncLock = Lockers.GetOrAdd(key, new AsyncLock());
            try
            {
                var releaser = asyncLock.Lock(source.Token);
                return new MemoryLocker(key, lockSecond, autoDelay, releaser);
            }
            catch (OperationCanceledException)
            {
                throw new LockFailureException(key);
            }
        }
    }
}