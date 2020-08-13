// Copyright � 2013-2015 Sergey Odinokov, Marco Casamento
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.Storage;
using Hangfire.Annotations;
using CSRedis;

namespace Hangfire.Redis
{
    internal class RedisConnection : JobStorageConnection
    {
        private readonly RedisStorage _storage;
        private readonly RedisSubscription _subscription;
        private readonly TimeSpan _fetchTimeout = TimeSpan.FromMinutes(3);
        public CSRedisClient RedisClient { get; }
        public RedisConnection(
            [NotNull] RedisStorage storage,
            CSRedisClient redisClient,
            [NotNull] RedisSubscription subscription,
            TimeSpan fetchTimeout)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _subscription = subscription ?? throw new ArgumentNullException(nameof(subscription));
            _fetchTimeout = fetchTimeout;
            RedisClient = redisClient;
        }

        public override IDisposable AcquireDistributedLock([NotNull] string resource, TimeSpan timeout)
        {
            //return RedisLock.Acquire(Redis, _storage.GetRedisKey(resource), timeout);
            return RedisClient.Lock(_storage.GetRedisKey(resource), (int)timeout.TotalSeconds);
        }

        public override void AnnounceServer([NotNull] string serverId, [NotNull] ServerContext context)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var pipe = RedisClient.StartPipe()
                   .SAdd(_storage.GetRedisKey("servers"), serverId)
                   .HMSet(_storage.GetRedisKey($"server:{serverId}"),
                       new Dictionary<string, string>
                       {
                            { "WorkerCount", context.WorkerCount.ToString(CultureInfo.InvariantCulture) },
                            { "StartedAt", JobHelper.SerializeDateTime(DateTime.UtcNow) },
                       }.DicToObjectArray()
                    );

            if (context.Queues.Length > 0)
                pipe.RPush(_storage.GetRedisKey($"server:{serverId}:queues"), context.Queues);

            pipe.EndPipe();
        }

        public override string CreateExpiredJob(
            [NotNull] Job job,
            [NotNull] IDictionary<string, string> parameters,
            DateTime createdAt,
            TimeSpan expireIn)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            var jobId = Guid.NewGuid().ToString("n");

            var invocationData = InvocationData.SerializeJob(job);

            // Do not modify the original parameters.
            var storedParameters = new Dictionary<string, string>(parameters)
            {
                { "Type", invocationData.Type },
                { "Method", invocationData.Method },
                { "ParameterTypes", invocationData.ParameterTypes },
                { "Arguments", invocationData.Arguments },
                { "CreatedAt", JobHelper.SerializeDateTime(createdAt) }
            };


            RedisClient.StartPipe()
                .HMSet(_storage.GetRedisKey($"job:{jobId}"), storedParameters.DicToObjectArray())
                .Expire(_storage.GetRedisKey($"job:{jobId}"), expireIn)
                .EndPipe();

            return jobId;
        }

        public override IWriteOnlyTransaction CreateWriteTransaction()
        {
            return new RedisWriteOnlyTransaction(_storage);
        }

        public override void Dispose()
        {
            // nothing to dispose
        }

        public override IFetchedJob FetchNextJob([NotNull] string[] queues, CancellationToken cancellationToken)
        {
            if (queues == null) throw new ArgumentNullException(nameof(queues));

            string jobId = null;
            string queueName = null;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                for (int i = 0; i < queues.Length; i++)
                {
                    queueName = queues[i];
                    var queueKey = _storage.GetRedisKey($"queue:{queueName}");
                    var fetchedKey = _storage.GetRedisKey($"queue:{queueName}:dequeued");
                    //jobId = Redis.ListRightPopLeftPush(queueKey, fetchedKey);
                    jobId = RedisClient.RPopLPush(queueKey, fetchedKey);
                    if (jobId != null) break;
                }

                if (jobId == null)
                {
                    _subscription.WaitForJob(_fetchTimeout, cancellationToken);
                }
            }
            while (jobId == null);

            // The job was fetched by the server. To provide reliability,
            // we should ensure, that the job will be performed and acquired
            // resources will be disposed even if the server will crash
            // while executing one of the subsequent lines of code.

            // The job's processing is splitted into a couple of checkpoints.
            // Each checkpoint occurs after successful update of the
            // job information in the storage. And each checkpoint describes
            // the way to perform the job when the server was crashed after
            // reaching it.

            // Checkpoint #1-1. The job was fetched into the fetched list,
            // that is being inspected by the FetchedJobsWatcher instance.
            // Job's has the implicit 'Fetched' state.

            //Redis.HashSet(
            //    _storage.GetRedisKey($"job:{jobId}"),
            //    "Fetched",
            //    JobHelper.SerializeDateTime(DateTime.UtcNow));

            RedisClient.HSet(_storage.GetRedisKey($"job:{jobId}"),
                "Fetched",
                JobHelper.SerializeDateTime(DateTime.UtcNow));

            // Checkpoint #2. The job is in the implicit 'Fetched' state now.
            // This state stores information about fetched time. The job will
            // be re-queued when the JobTimeout will be expired.

            return new RedisFetchedJob(_storage, RedisClient, jobId, queueName);
        }

        public override Dictionary<string, string> GetAllEntriesFromHash([NotNull] string key)
        {
            //var result = Redis.HashGetAll(_storage.GetRedisKey(key)).ToStringDictionary();
            var result = RedisClient.HGetAll(_storage.GetRedisKey(key));
            return result.Count != 0 ? result : null;
        }

        public override List<string> GetAllItemsFromList([NotNull] string key)
        {
            return RedisClient.LRange(_storage.GetRedisKey(key), 0, -1).ToList();
            //return Redis.ListRange(_storage.GetRedisKey(key)).ToStringArray().ToList();
        }

        public override HashSet<string> GetAllItemsFromSet([NotNull] string key)
        {
            HashSet<string> result = new HashSet<string>();
            //foreach (var item in Redis.SortedSetScan(_storage.GetRedisKey(key)))
            foreach (var item in RedisClient.ZScan(_storage.GetRedisKey(key), 0).Items)
            {
                result.Add(item.member);
            }

            return result;
        }

        public override long GetCounter([NotNull] string key)
        {
            //return Convert.ToInt64(Redis.StringGet(_storage.GetRedisKey(key)));
            return Convert.ToInt64(RedisClient.Get(_storage.GetRedisKey(key)));
        }

        public override string GetFirstByLowestScoreFromSet([NotNull] string key, double fromScore, double toScore)
        {
            //return Redis.SortedSetRangeByScore(_storage.GetRedisKey(key), fromScore, toScore, skip: 0, take: 1)
            //    .FirstOrDefault();

            return RedisClient.ZRangeByScore(_storage.GetRedisKey(key),
                (decimal)fromScore, (decimal)toScore, 1, 0).FirstOrDefault();
        }

        public override long GetHashCount([NotNull] string key)
        {
            //return Redis.HashLength(_storage.GetRedisKey(key));
            return RedisClient.HLen(_storage.GetRedisKey(key));
        }

        public override TimeSpan GetHashTtl([NotNull] string key)
        {
            //return Redis.KeyTimeToLive(_storage.GetRedisKey(key)) ?? TimeSpan.Zero;

            return TimeSpan.FromSeconds(RedisClient.Ttl(key));
        }

        public override JobData GetJobData([NotNull] string jobId)
        {
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));

            //var storedData = Redis.HashGetAll(_storage.GetRedisKey($"job:{jobId}"));
            var storedData = RedisClient.HGetAll(_storage.GetRedisKey($"job:{jobId}"));
            if (storedData.Count == 0) return null;

            string type = storedData.FirstOrDefault(x => x.Key == "Type").Value;
            string method = storedData.FirstOrDefault(x => x.Key == "Method").Value;
            string parameterTypes = storedData.FirstOrDefault(x => x.Key == "ParameterTypes").Value;
            string arguments = storedData.FirstOrDefault(x => x.Key == "Arguments").Value;
            string createdAt = storedData.FirstOrDefault(x => x.Key == "CreatedAt").Value;

            Job job = null;
            JobLoadException loadException = null;

            var invocationData = new InvocationData(type, method, parameterTypes, arguments);

            try
            {
                job = invocationData.DeserializeJob();
            }
            catch (JobLoadException ex)
            {
                loadException = ex;
            }

            return new JobData
            {
                Job = job,
                State = storedData.FirstOrDefault(x => x.Key == "State").Value,
                CreatedAt = JobHelper.DeserializeNullableDateTime(createdAt) ?? DateTime.MinValue,
                LoadException = loadException
            };
        }

        public override string GetJobParameter([NotNull] string jobId, [NotNull] string name)
        {
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));
            if (name == null) throw new ArgumentNullException(nameof(name));

            //return Redis.HashGet(_storage.GetRedisKey($"job:{jobId}"), name);
            return RedisClient.HGet(_storage.GetRedisKey($"job:{jobId}"), name);
        }

        public override long GetListCount([NotNull] string key)
        {
            //return Redis.ListLength(_storage.GetRedisKey(key));
            return RedisClient.LLen(_storage.GetRedisKey(key));
        }

        public override TimeSpan GetListTtl([NotNull] string key)
        {
            //return Redis.KeyTimeToLive(_storage.GetRedisKey(key)) ?? TimeSpan.Zero;
            return TimeSpan.FromSeconds(RedisClient.Ttl(_storage.GetRedisKey(key)));
        }

        public override List<string> GetRangeFromList([NotNull] string key, int startingFrom, int endingAt)
        {
            //return Redis.ListRange(_storage.GetRedisKey(key), startingFrom, endingAt).ToStringArray().ToList();
            return RedisClient.LRange(_storage.GetRedisKey(key), startingFrom, endingAt).ToList();
        }

        public override List<string> GetRangeFromSet([NotNull] string key, int startingFrom, int endingAt)
        {
            //return Redis.SortedSetRangeByRank(_storage.GetRedisKey(key), startingFrom, endingAt).ToStringArray().ToList();
            return RedisClient.ZRange(_storage.GetRedisKey(key), startingFrom, endingAt).ToList();
        }

        public override long GetSetCount([NotNull] string key)
        {
            //return Redis.SortedSetLength(_storage.GetRedisKey(key));
            return RedisClient.ZCard(_storage.GetRedisKey(key));
        }

        public override TimeSpan GetSetTtl([NotNull] string key)
        {
            //return Redis.KeyTimeToLive(_storage.GetRedisKey(key)) ?? TimeSpan.Zero;
            return TimeSpan.FromSeconds(RedisClient.Ttl(_storage.GetRedisKey(key)));
        }

        public override StateData GetStateData([NotNull] string jobId)
        {
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));

            //var entries = Redis.HashGetAll(_storage.GetRedisKey($"job:{jobId}:state"));
            var entries = RedisClient.HGetAll(_storage.GetRedisKey($"job:{jobId}:state"));
            if (entries.Count == 0) return null;

            //var stateData = entries.ToStringDictionary();
            string name = entries["State"];
            entries.TryGetValue("Reason", out var reason);
            entries.Remove("State");
            entries.Remove("Reason");

            return new StateData
            {
                Name = name,
                Reason = reason,
                Data = entries
            };
        }

        public override string GetValueFromHash([NotNull] string key, [NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            //return Redis.HashGet(_storage.GetRedisKey(key), name);
            return RedisClient.HGet(_storage.GetRedisKey(key), name);
        }

        public override void Heartbeat([NotNull] string serverId)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));

            //Redis.HashSet(
            //    _storage.GetRedisKey($"server:{serverId}"),
            //    "Heartbeat",
            //    JobHelper.SerializeDateTime(DateTime.UtcNow));

            RedisClient.HSet(
                _storage.GetRedisKey($"server:{serverId}"),
                "Heartbeat",
                JobHelper.SerializeDateTime(DateTime.UtcNow));
        }

        public override void RemoveServer([NotNull] string serverId)
        {
            if (serverId == null) throw new ArgumentNullException(nameof(serverId));

            //var transaction = Redis.CreateTransaction();

            //transaction.SetRemoveAsync(_storage.GetRedisKey("servers"), serverId);



            //transaction.KeyDeleteAsync(
            //    new RedisKey[]
            //    {
            //        _storage.GetRedisKey($"server:{serverId}"),
            //        _storage.GetRedisKey($"server:{serverId}:queues")
            //    });

            //transaction.Execute();

            RedisClient.StartPipe().SRem(_storage.GetRedisKey("servers"), serverId)
                .Del(_storage.GetRedisKey($"server:{serverId}"))
                .Del(_storage.GetRedisKey($"server:{serverId}:queues"))
                .EndPipe();
        }

        public override int RemoveTimedOutServers(TimeSpan timeOut)
        {
            //var serverNames = Redis.SetMembers(_storage.GetRedisKey("servers"));
            var serverNames = RedisClient.SMembers(_storage.GetRedisKey("servers"));
            var heartbeats = new Dictionary<string, Tuple<DateTime, DateTime?>>();

            var utcNow = DateTime.UtcNow;

            foreach (var serverName in serverNames)
            {
                //var srv = Redis.HashGet(_storage.GetRedisKey($"server:{serverName}"), new RedisValue[] { "StartedAt", "Heartbeat" });
                var srv = RedisClient.HMGet(_storage.GetRedisKey($"server:{serverName}"), new string[] { "StartedAt", "Heartbeat" });
                heartbeats.Add(serverName,
                                new Tuple<DateTime, DateTime?>(
                                JobHelper.DeserializeDateTime(srv[0]),
                                JobHelper.DeserializeNullableDateTime(srv[1])));
            }

            var removedServerCount = 0;
            foreach (var heartbeat in heartbeats)
            {
                var maxTime = new DateTime(
                    Math.Max(heartbeat.Value.Item1.Ticks, (heartbeat.Value.Item2 ?? DateTime.MinValue).Ticks));

                if (utcNow > maxTime.Add(timeOut))
                {
                    RemoveServer(heartbeat.Key);
                    removedServerCount++;
                }
            }

            return removedServerCount;
        }

        public override void SetJobParameter([NotNull] string jobId, [NotNull] string name, string value)
        {
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));
            if (name == null) throw new ArgumentNullException(nameof(name));

            //Redis.HashSet(_storage.GetRedisKey($"job:{jobId}"), name, value);
            RedisClient.HSet(_storage.GetRedisKey($"job:{jobId}"), name, value);
        }

        public override void SetRangeInHash([NotNull] string key, [NotNull] IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            if (keyValuePairs == null) throw new ArgumentNullException(nameof(keyValuePairs));

            //Redis.HashSet(_storage.GetRedisKey(key), keyValuePairs.ToHashEntries());
            RedisClient.HMSet(_storage.GetRedisKey(key), keyValuePairs.DicToObjectArray());
        }
    }
}
