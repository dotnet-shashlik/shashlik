using System;
using System.Threading;
using CSRedis;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    public class CSRedisLocker : IDisposable
    {
        public CSRedisLocker(CSRedisClient redisClient, bool autoDelay, int lockSecond, string key, string value)
        {
            RedisClient = redisClient;
            LockSecond = lockSecond;
            Key = key;
            Value = value;

            if (autoDelay)
            {
                CancelTokenSource = new CancellationTokenSource();
                TimerHelper.SetInterval(Delay, TimeSpan.FromSeconds(LockSecond / 2D), CancelTokenSource.Token);
            }
        }

        private CSRedisClient RedisClient { get; }

        private int LockSecond { get; }

        private void Delay()
        {
            var value = RedisClient.Get(Key);
            var b = false;
            if (value == Value)
                b = RedisClient.Set(Key, Value, LockSecond, RedisExistence.Xx);

            if (!b)
                Dispose();
        }

        private string Key { get; }

        private string Value { get; }

        private CancellationTokenSource CancelTokenSource { get; }

        public void Dispose()
        {
            try
            {
                RedisClient.Del(Key);
                CancelTokenSource?.Cancel();
                CancelTokenSource?.Dispose();
            }
            catch
            {
                // ignored
            }
        }
    }
}