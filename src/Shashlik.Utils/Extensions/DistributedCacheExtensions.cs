using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Shashlik.Utils.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var content = await cache.GetStringAsync(key);
            return content.IsNullOrWhiteSpace() ? default : JsonSerializer.Deserialize<T>(content);
        }

        public static async Task SetObjectAsync(this IDistributedCache cache, string key, object obj,
            DateTimeOffset? expireAt)
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireAt
            });
        }

        public static T GetObject<T>(this IDistributedCache cache, string key)
        {
            var content = cache.GetString(key);
            return content.IsNullOrWhiteSpace() ? default : JsonSerializer.Deserialize<T>(content);
        }

        public static void SetObject(this IDistributedCache cache, string key, object obj,
            DateTimeOffset? expireAt)
        {
            cache.SetString(key, JsonSerializer.Serialize(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireAt
            });
        }
    }
}