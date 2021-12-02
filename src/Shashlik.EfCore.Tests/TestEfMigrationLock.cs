using Shashlik.EfCore.Migration;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.EfCore.Tests
{
    [Singleton]
    internal class TestEfMigrationLock : IEfMigrationLock, IDisposable
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
