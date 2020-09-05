using System;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Locker.Memory
{
    public class MemoryLocker : IDisposable
    {
        public MemoryLocker(string key, int lockSecond, bool autoDelay, AsyncLock.Releaser releaser)
        {
            LockSecond = lockSecond;
            Key = key;
            AutoDelay = autoDelay;
            Releaser = releaser;

            TimerHelper.SetTimeout(this.Release, TimeSpan.FromSeconds(lockSecond));
        }

        private AsyncLock.Releaser Releaser { get; }

        private string Key { get; }

        private int LockSecond { get; }

        private bool AutoDelay { get; }

        public bool Disposed { get; private set; }

        private void Release()
        {
            if (Disposed)
                return;
            if (!AutoDelay)
                this.Dispose();
        }

        public void Dispose()
        {
            try
            {
                Disposed = true;
                Releaser.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}