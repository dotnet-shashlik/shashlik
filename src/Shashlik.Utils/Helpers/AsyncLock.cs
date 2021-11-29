using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 异步内存锁
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// lock
        /// </summary>
        /// <param name="waitTimeoutCancellationToken">锁等待超时token</param>
        /// <returns></returns>
        public async Task<Releaser> LockAsync(CancellationToken waitTimeoutCancellationToken = default)
        {
            waitTimeoutCancellationToken.ThrowIfCancellationRequested();
            await _semaphore.WaitAsync(waitTimeoutCancellationToken);
            return new Releaser(this);
        }

        /// <summary>
        /// lock
        /// </summary>
        /// <param name="waitTimeoutCancellationToken">锁等待超时token</param>
        /// <returns></returns>
        public Releaser Lock(CancellationToken waitTimeoutCancellationToken = default)
        {
            waitTimeoutCancellationToken.ThrowIfCancellationRequested();
            _semaphore.Wait(waitTimeoutCancellationToken);
            return new Releaser(this);
        }

        public void Dispose()
        {
            using (_semaphore)
            {
            }
        }

        public readonly struct Releaser : IDisposable
        {
            private readonly AsyncLock _toRelease;

            internal Releaser(AsyncLock toRelease)
            {
                _toRelease = toRelease;
            }

            public void Dispose()
                => _toRelease._semaphore.Release();
        }
    }
}