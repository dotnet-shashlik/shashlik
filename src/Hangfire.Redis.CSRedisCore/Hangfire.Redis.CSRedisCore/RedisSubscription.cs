using System;
using System.Threading;
using Hangfire.Server;
using Hangfire.Annotations;
using CSRedis;
using static CSRedis.CSRedisClient;

namespace Hangfire.Redis
{
#pragma warning disable 618
    internal class RedisSubscription : IServerComponent
#pragma warning restore 618
    {
        private readonly ManualResetEvent _mre = new ManualResetEvent(false);
        private readonly RedisStorage _storage;
        private readonly CSRedisClient _redisClient;
        private readonly SubscribeObject subscribeObject;
        public RedisSubscription([NotNull] RedisStorage storage, [NotNull] CSRedisClient redisClient)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Channel = _storage.GetRedisKey("JobFetchChannel");

            _redisClient = redisClient ?? throw new ArgumentNullException(nameof(redisClient));
            subscribeObject = _redisClient.Subscribe((Channel, r => _mre.Set()));
        }

        public string Channel { get; }

        public void WaitForJob(TimeSpan timeout, CancellationToken cancellationToken)
        {
            _mre.Reset();
            WaitHandle.WaitAny(new[] { _mre, cancellationToken.WaitHandle }, timeout);
        }

        void IServerComponent.Execute(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();

            if (cancellationToken.IsCancellationRequested)
            {
                //_subscriber.Unsubscribe(Channel);
                subscribeObject.Unsubscribe();
                _mre.Dispose();
                subscribeObject.Dispose();
            }
        }
    }
}
