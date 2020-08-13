﻿// Copyright © 2013-2015 Sergey Odinokov, Marco Casamento
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
using Hangfire.Storage;
using Hangfire.Annotations;

namespace Hangfire.Redis
{
    internal class RedisFetchedJob : IFetchedJob
    {
        private readonly RedisStorage _storage;
        private readonly CSRedis.CSRedisClient _redisClient;
        private bool _disposed;
        private bool _removedFromQueue;
        private bool _requeued;

        public RedisFetchedJob(
            [NotNull] RedisStorage storage,
            [NotNull] CSRedis.CSRedisClient redisClient,
            [NotNull] string jobId,
            [NotNull] string queue)
        {
            if (storage == null) throw new ArgumentNullException(nameof(storage));
            if (redisClient == null) throw new ArgumentNullException(nameof(redisClient));
            if (jobId == null) throw new ArgumentNullException(nameof(jobId));
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            _storage = storage;
            _redisClient = redisClient;
            JobId = jobId;
            Queue = queue;
        }

        public string JobId { get; }
        public string Queue { get; }

        public void RemoveFromQueue()
        {
            RemoveFromFetchedList();
            _removedFromQueue = true;
        }

        public void Requeue()
        {
            _redisClient.RPush(_storage.GetRedisKey($"queue:{Queue}"), JobId);
            RemoveFromFetchedList();

            _requeued = true;
        }

        public void Dispose()
        {
            if (_disposed) return;

            if (!_removedFromQueue && !_requeued)
            {
                Requeue();
            }

            _disposed = true;
        }

        private void RemoveFromFetchedList()
        {
            var obj = _redisClient.StartPipe()
               .LRem(_storage.GetRedisKey($"queue:{Queue}:dequeued"), -1, JobId)
               .HDel(_storage.GetRedisKey($"job:{JobId}"), new string[] { "Fetched", "Checked" })
               .EndPipe();

            //databaseAsync.ListRemoveAsync(_storage.GetRedisKey($"queue:{Queue}:dequeued"), JobId, -1);
            //databaseAsync.HashDeleteAsync(_storage.GetRedisKey($"job:{JobId}"), new RedisValue[] { "Fetched", "Checked" });
        }
    }
}
