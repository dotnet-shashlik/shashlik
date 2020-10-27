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
        public void CancelAsyncTest()
        {
            var locker = new AsyncLock();

            // 首次上锁2秒
            {
                using var cancelToken = new CancellationTokenSource();
                cancelToken.CancelAfter(TimeSpan.FromSeconds(1));
                var release = locker.LockAsync(cancelToken.Token).GetAwaiter().GetResult();

                Task.Run(async () =>
                {
                    // 2秒后释放锁
                    await Task.Delay(2000);
                    release.Dispose();
                });
            }

            // 再来锁,等待0.5秒,应该异常
            {
                using var cancelToken = new CancellationTokenSource();
                cancelToken.CancelAfter(TimeSpan.FromSeconds(0.5));
                // 锁不上,异常
                Should.Throw<Exception>(() => { locker.LockAsync(cancelToken.Token).Wait(); });
            }

            // 2秒后可以锁上
            {
                Task.Delay(2000).Wait();
                using var cancelToken = new CancellationTokenSource();
                cancelToken.CancelAfter(TimeSpan.FromSeconds(0.5));
                locker.LockAsync().Wait();
            }
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
            Should.Throw<Exception>(() => { locker.Lock(cancelToken.Token); });
            number.ShouldBe(2);
        }
    }
}