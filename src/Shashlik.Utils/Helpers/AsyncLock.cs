using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shashlik.Utils.Helpers
{
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public async Task<Releaser> LockAsync(CancellationToken waitTimeoutCancellationToken = default)
        {
            waitTimeoutCancellationToken.ThrowIfCancellationRequested();
            await _semaphore.WaitAsync(waitTimeoutCancellationToken);
            return new Releaser(this);
        }

        public Releaser Lock(CancellationToken waitTimeoutCancellationToken = default)
        {
            waitTimeoutCancellationToken.ThrowIfCancellationRequested();
            _semaphore.Wait(waitTimeoutCancellationToken);
            return new Releaser(this);
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