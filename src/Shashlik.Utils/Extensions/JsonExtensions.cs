using System;
using Newtonsoft.Json;

namespace Shashlik.Utils.Extensions
{
    /// <summary>
    /// 使用Newtonsoft.Json
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// json反序列化(Newtonsoft.Json)
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? DeserializeJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// json序列化(Newtonsoft.Json)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerSettings? serializerSettings = null)
            where T : class
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));

            return JsonConvert.SerializeObject(obj, serializerSettings ?? new JsonSerializerSettings());
        }

        /// <summary>
        /// json序列化,小驼峰命名(Newtonsoft.Json)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonWithCamelCase<T>(this T obj)
            where T : class
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }
    }
}