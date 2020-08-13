﻿using System;
using System.Threading;
using Hangfire.Common;
using Xunit;
using System.Linq;

namespace Hangfire.Redis.Tests
{
    [CleanRedis]
    public class FetchedJobsWatcherFacts
    {
        private static readonly TimeSpan InvisibilityTimeout = TimeSpan.FromSeconds(10);

        private readonly RedisStorage _storage;
        private readonly CancellationTokenSource _cts;

        public FetchedJobsWatcherFacts()
        {
            var options = new RedisStorageOptions() { };
            _storage = new RedisStorage(RedisUtils.RedisClient, options);
            _cts = new CancellationTokenSource();
            _cts.Cancel();
        }

        [Fact]
        public void Ctor_ThrowsAnException_WhenStorageIsNull()
        {
            Assert.Throws<ArgumentNullException>("storage",
                () => new FetchedJobsWatcher(null, InvisibilityTimeout));
        }

        [Fact]
        public void Ctor_ThrowsAnException_WhenInvisibilityTimeoutIsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>("invisibilityTimeout",
                () => new FetchedJobsWatcher(_storage, TimeSpan.Zero));
        }

        [Fact]
        public void Ctor_ThrowsAnException_WhenInvisibilityTimeoutIsNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>("invisibilityTimeout",
                () => new FetchedJobsWatcher(_storage, TimeSpan.FromSeconds(-1)));
        }

        [Fact]
        public void Execute_EnqueuesTimedOutJobs_AndDeletesThemFromFetchedList()
        {
            var redis = RedisUtils.RedisClient;
            // Arrange
            redis.SAdd("{hangfire}:queues", "my-queue");
            redis.RPush("{hangfire}:queue:my-queue:dequeued", "my-job");
            redis.HSet("{hangfire}:job:my-job", "Fetched",
                JobHelper.SerializeDateTime(DateTime.UtcNow.AddDays(-1)));

            var watcher = CreateWatcher();

            // Act
            watcher.Execute(_cts.Token);

            // Assert
            Assert.Equal(0, redis.LLen("{hangfire}:queue:my-queue:dequeued"));

            var listEntry = (string)redis.RPop("{hangfire}:queue:my-queue");
            Assert.Equal("my-job", listEntry);

            var job = redis.HGetAll("{hangfire}:job:my-job");
            Assert.DoesNotContain(job, x => x.Key == "Fetched");
        }

        [Fact, CleanRedis]
        public void Execute_MarksDequeuedJobAsChecked_IfItHasNoFetchedFlagSet()
        {
            var redis = RedisUtils.RedisClient;
            // Arrange
            redis.SAdd("{hangfire}:queues", "my-queue");
            redis.RPush("{hangfire}:queue:my-queue:dequeued", "my-job");

            var watcher = CreateWatcher();

            // Act
            watcher.Execute(_cts.Token);

            Assert.NotNull(JobHelper.DeserializeNullableDateTime(
                redis.HGet("{hangfire}:job:my-job", "Checked")));
        }

        [Fact, CleanRedis]
        public void Execute_EnqueuesCheckedAndTimedOutJob_IfNoFetchedFlagSet()
        {
            var redis = RedisUtils.RedisClient;
            // Arrange
            redis.SAdd("{hangfire}:queues", "my-queue");
            redis.RPush("{hangfire}:queue:my-queue:dequeued", "my-job");
            redis.HSet("{hangfire}:job:my-job", "Checked",
                JobHelper.SerializeDateTime(DateTime.UtcNow.AddDays(-1)));

            var watcher = CreateWatcher();

            // Act
            watcher.Execute(_cts.Token);

            // Arrange
            Assert.Equal(0, redis.LLen("{hangfire}:queue:my-queue:dequeued"));
            Assert.Equal(1, redis.LLen("{hangfire}:queue:my-queue"));

            var job = redis.HGetAll("{hangfire}:job:my-job");
            Assert.DoesNotContain(job, x => x.Key == "Checked");
        }

        [Fact, CleanRedis]
        public void Execute_DoesNotEnqueueTimedOutByCheckedFlagJob_IfFetchedFlagSet()
        {
            var redis = RedisUtils.RedisClient;

            // Arrange
            redis.SAdd("{hangfire}:queues", "my-queue");
            redis.RPush("{hangfire}:queue:my-queue:dequeued", "my-job");
            redis.HSet("{hangfire}:job:my-job", "Checked",
                JobHelper.SerializeDateTime(DateTime.UtcNow.AddDays(-1)));
            redis.HSet("{hangfire}:job:my-job", "Fetched",
                JobHelper.SerializeDateTime(DateTime.UtcNow));

            var watcher = CreateWatcher();

            // Act
            watcher.Execute(_cts.Token);

            // Assert
            Assert.Equal(1, redis.LLen("{hangfire}:queue:my-queue:dequeued"));

        }

        private FetchedJobsWatcher CreateWatcher()
        {
            return new FetchedJobsWatcher(_storage, InvisibilityTimeout);
        }
    }
}
