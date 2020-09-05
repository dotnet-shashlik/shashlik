using System;
using System.Threading;
using System.Threading.Tasks;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
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

        [Fact]
        public async Task CancelAsyncTest()
        {
            var locker = new AsyncLock();
            var number = 1;
            var cancelToken = new CancellationTokenSource();
            await locker.LockAsync();
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                number = 2;
                cancelToken.Cancel();
            });
            Should.Throw<Exception>(() =>
            {
                locker.LockAsync(cancelToken.Token).Wait();
            });
            number.ShouldBe(2);
        }

        [Fact]
        public void CancelTest()
        {
            var locker = new AsyncLock();
            var number = 1;
            var cancelToken = new CancellationTokenSource();
            locker.Lock();
            Task.Run(async () =>
            {
                await Task.Delay(10000);
                number = 2;
                cancelToken.Cancel();
            });
            Should.Throw<Exception>(() =>
            {
                locker.Lock(cancelToken.Token);
            });
            number.ShouldBe(2);
        }
    }
}