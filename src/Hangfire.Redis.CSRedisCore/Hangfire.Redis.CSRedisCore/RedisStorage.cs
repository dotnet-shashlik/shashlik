// Copyright ?2013-2015 Sergey Odinokov, Marco Casamento
// This software is based on https://github.com/HangfireIO/Hangfire.Redis

// Hangfire.Redis.StackExchange is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as
// published by the Free Software Foundation, either version 3
// of the License, or any later version.
//
// Hangfire.Redis.StackExchange is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with Hangfire.Redis.StackExchange. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Collections.Generic;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Hangfire.Annotations;
using Hangfire.Logging;
using Hangfire.Dashboard;

namespace Hangfire.Redis
{
    public class RedisStorage : JobStorage
    {
        // Make sure in Redis Cluster all transaction are in the same slot !!
        private readonly RedisStorageOptions _options;
        private readonly RedisSubscription _subscription;
        public CSRedis.CSRedisClient RedisClient { get; }

        public RedisStorage(CSRedis.CSRedisClient redisClient, RedisStorageOptions options = null)
        {
            RedisClient = redisClient;
            this._options = options ?? new RedisStorageOptions();
            this._subscription = new RedisSubscription(this, redisClient);
        }


        internal int SucceededListSize => _options.SucceededListSize;

        internal int DeletedListSize => _options.DeletedListSize;

        internal string SubscriptionChannel => _subscription.Channel;

        internal string[] LifoQueues => _options.LifoQueues;

        //internal bool UseTransactions => _options.UseTransactions;

        public override IMonitoringApi GetMonitoringApi()
        {
            return new RedisMonitoringApi(this, RedisClient);
        }

        public override IStorageConnection GetConnection()
        {
            return new RedisConnection(this, RedisClient, _subscription, _options.FetchTimeout);
        }

#pragma warning disable 618
        public override IEnumerable<IServerComponent> GetComponents()
#pragma warning restore 618
        {
            yield return new FetchedJobsWatcher(this, _options.InvisibilityTimeout);
            yield return new ExpiredJobsWatcher(this, _options.ExpiryCheckInterval);
            yield return _subscription;
        }

        public static DashboardMetric GetDashboardMetricFromRedisInfo(string title, string key)
        {
            return new DashboardMetric("redis:" + key, title, (razorPage) =>
            {
                using (var redisCnn = razorPage.Storage.GetConnection())
                {
                    //var db = (redisCnn as RedisConnection).Redis;
                    //var cnnMultiplexer = db.Multiplexer;
                    //var srv = cnnMultiplexer.GetServer(db.IdentifyEndpoint());
                    //var rawInfo = srv.InfoRaw().Split('\n')
                    //    .Where(x => x.Contains(':'))
                    //    .ToDictionary(x => x.Split(':')[0], x => x.Split(':')[1]);

                    //TODO: ÑéÖ¤
                    var rawInfo = ((redisCnn as RedisConnection).RedisClient)
                    .NodesServerManager.Info()
                    .ToDictionary(r => r.node, r => r.value);

                    return new Metric(rawInfo[key]);
                }
            });
        }

        public override IEnumerable<IStateHandler> GetStateHandlers()
        {
            yield return new FailedStateHandler();
            yield return new ProcessingStateHandler();
            yield return new SucceededStateHandler();
            yield return new DeletedStateHandler();
        }

        public override void WriteOptionsToLog(ILog logger)
        {
            logger.Debug("Using the following options for Redis job storage:");

            //logger.DebugFormat("ConnectionString: {0}\nDN: {1}", ConnectionString, Db);
        }

        public override string ToString()
        {
            return RedisClient.ToString();
            //return string.Format("redis://{0}/{1}", ConnectionString, Db);
        }

        internal string GetRedisKey([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return _options.Prefix + key;
        }
    }
}