using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class DistributedCacheTests
    {
        public class MemoryCache : IDistributedCache
        {
            private Dictionary<string, byte[]> Cache { get; } = new Dictionary<string, byte[]>();
            public byte[] Get(string key)
            {
                return Cache.GetOrDefault(key);
            }

            public Task<byte[]> GetAsync(string key, CancellationToken token = new CancellationToken())
            {
                return Task.FromResult(Cache.GetOrDefault(key));
            }

            public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            {
                Cache[key] = value;
            }

            public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options,
                CancellationToken token = new CancellationToken())
            {
                Cache[key] = value;
                return Task.CompletedTask;
            }

            public void Refresh(string key)
            {
                return;
            }

            public Task RefreshAsync(string key, CancellationToken token = new CancellationToken())
            {
                return Task.CompletedTask;
            }

            public void Remove(string key)
            {
                Cache.Remove(key);
            }

            public Task RemoveAsync(string key, CancellationToken token = new CancellationToken())
            {
                Cache.Remove(key);
                return Task.CompletedTask;
            }
        }

        private class TestClass
        {
            public string Str { get; set; }
        }

        [Fact]
        public void Tests()
        {
            var cache = new MemoryCache();
            var obj = new TestClass() {Str = "test"};
            cache.SetObject("test", obj , DateTimeOffset.Now.AddSeconds(5));
            var cacheObj = cache.GetObject<TestClass>("test");
            cacheObj.ShouldNotBeNull();
            cacheObj.Str.ShouldBe(obj.Str);
        }

        [Fact]
        public async Task AsyncTests()
        {
            var cache = new MemoryCache();
            var obj = new TestClass() {Str = "test"};
            await cache.SetObjectAsync("test", obj , DateTimeOffset.Now.AddSeconds(5));
            var cacheObj = await cache.GetObjectAsync<TestClass>("test");
            cacheObj.ShouldNotBeNull();
            cacheObj.Str.ShouldBe(obj.Str);
        }
    }
}