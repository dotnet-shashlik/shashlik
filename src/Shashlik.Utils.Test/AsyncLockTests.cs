using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shashlik.Utils.Common;
using Shouldly;

namespace Shashlik.Utils.Test
{
    public class AsyncLockTests
    {
        [Fact]
        async Task lockAsyncTest()
        {
            var a = Guid.NewGuid().ToString("N");

            AsyncLock locker = new AsyncLock();
            int counter = 1;
            await Task.CompletedTask;
            Parallel.For(0, 100, r =>
            {
                using (var releaser = locker.LockAsync().GetAwaiter().GetResult())
                    counter++;
            });

            counter.ShouldBe(1 + 100);
        }

        [Fact]
        void lockTest()
        {
            AsyncLock locker = new AsyncLock();
            int counter = 1;
            Parallel.For(0, 100, r =>
            {
                using (var releaser = locker.Lock())
                    counter++;
            });
            counter.ShouldBe(1 + 100);
        }
    }
}
