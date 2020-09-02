using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shashlik.Utils.Helpers;
using Xunit;
using Shouldly;

namespace Shashlik.Utils.Test
{
    public class AsyncLockTests
    {
        [Fact]
        public async Task LockAsyncTest()
        {
            var a = Guid.NewGuid().ToString("N");

            var locker = new AsyncLock();
            var counter = 1;
            await Task.CompletedTask;
            Parallel.For(0, 100, r =>
            {
                using var releaser = locker.LockAsync().GetAwaiter().GetResult();
                counter++;
            });

            counter.ShouldBe(1 + 100);
        }

        [Fact]
        public void LockTest()
        {
            var locker = new AsyncLock();
            var counter = 1;
            Parallel.For(0, 100, r =>
            {
                using var releaser = locker.Lock();
                counter++;
            });
            counter.ShouldBe(1 + 100);
        }
    }
}