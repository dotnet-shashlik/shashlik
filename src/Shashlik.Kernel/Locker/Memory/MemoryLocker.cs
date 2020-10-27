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

            if (!autoDelay)
            {
                TimerHelper.SetTimeout(Dispose, TimeSpan.FromSeconds(lockSecond));
            }
        }

        private AsyncLock.Releaser Releaser { get; }

        private string Key { get; }

        private int LockSecond { get; }

        private bool AutoDelay { get; }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            try
            {
                if (!Disposed)
                {
                    Disposed = true;
                    Releaser.Dispose();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}