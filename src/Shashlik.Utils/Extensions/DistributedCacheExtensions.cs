using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

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
        public static async Task<T> GetObjectWithJsonAsync<T>(this IDistributedCache cache, string key)
        {
            var content = await cache.GetStringAsync(key);
            return string.IsNullOrWhiteSpace(content) ? default! : JsonSerializer.Deserialize<T>(content);
        }

        /// <summary>
        /// 设置json数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">环境key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expireAt">绝对过期时间,null不过期</param>
        /// <returns></returns>
        public static async Task SetObjectWithJsonAsync(this IDistributedCache cache, string key, object obj,
            DateTimeOffset? expireAt)
        {
            await cache.SetStringAsync(key, JsonSerializer.Serialize(obj), new DistributedCacheEntryOptions
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
        public static T GetObjectWithJson<T>(this IDistributedCache cache, string key)
        {
            var content = cache.GetString(key);
            return string.IsNullOrWhiteSpace(content) ? default! : JsonSerializer.Deserialize<T>(content);
        }

        /// <summary>
        /// 设置json数据缓存
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key">环境key</param>
        /// <param name="obj">缓存对象</param>
        /// <param name="expireAt">绝对过期时间,null不过期</param>
        /// <returns></returns>
        public static void SetObjectWithJson(this IDistributedCache cache, string key, object obj,
            DateTimeOffset? expireAt)
        {
            cache.SetString(key, JsonSerializer.Serialize(obj), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expireAt
            });
        }
    }
}