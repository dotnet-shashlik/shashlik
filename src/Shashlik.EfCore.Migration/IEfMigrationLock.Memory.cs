using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Helpers;
using System;

namespace Shashlik.EfCore.Migration
{
    [Singleton]
    [ConditionDependsOnMissing(typeof(IEfMigrationLock))]
    public class MemoryEfCoreMigrationLock : IEfMigrationLock, IDisposable
    {
        private readonly AsyncLock _locker = new AsyncLock();

        public void Dispose()
        {
            _locker.Dispose();
        }

        public IDisposable Lock()
        {
            return _locker.Lock();
        }
    }
}