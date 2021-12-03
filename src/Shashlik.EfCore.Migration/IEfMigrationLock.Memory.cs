using Shashlik.EfCore.Migration;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Helpers;
using System;

namespace Shashlik.EfCore.Migration
{
    [Singleton]
    [ConditionDependsOnMissing(typeof(IEfMigrationLock))]
    internal class MemoryEfCoreMigrationLock : IEfMigrationLock, IDisposable
    {
        private readonly AsyncLock Locker = new AsyncLock();

        public void Dispose()
        {
            Locker.Dispose();
        }

        public IDisposable Lock()
        {
            return Locker.Lock();
        }
    }
}
