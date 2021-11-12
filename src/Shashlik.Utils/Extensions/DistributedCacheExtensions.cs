using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Shashlik.Utils.Extensions
{
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// 从json格式的缓存数据中获取对象示例
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">缓存key</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T?> GetObjectWithJsonAsync<T>(this IDistributedCache cache, string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            var content = await cache.GetStringAsync(key);
            return string.IsNullOrWhiteSpace(content) ? default : JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// 设置json数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">环境key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expireAt">绝对过期时间,null不过期</param>
        /// <returns></returns>
        public static async Task SetObjectWithJsonAsync(this IDistributedCache cache, string key, object? obj,
            DateTimeOffset? expireAt)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            await cache.SetStringAsync(key, JsonConvert.SerializeObject(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireAt
            });
        }

        /// <summary>
        /// 从json格式的缓存数据中获取对象示例
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">缓存key</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetObjectWithJson<T>(this IDistributedCache cache, string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            var content = cache.GetString(key);
            return string.IsNullOrWhiteSpace(content) ? default : JsonConvert.DeserializeObject<T>(content);
        }

        /// <summary>
        /// 设置json数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">环境key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expireAt">绝对过期时间,null不过期</param>
        /// <returns></returns>
        public static void SetObjectWithJson(this IDistributedCache cache, string key, object? obj,
            DateTimeOffset? expireAt)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            cache.SetString(key, JsonConvert.SerializeObject(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireAt
            });
        }
    }
}