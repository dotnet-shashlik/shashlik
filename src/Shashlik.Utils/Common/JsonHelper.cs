using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.Utils.Common
{
    public static class JsonHelper
    {
        /// <summary>
        /// 默认的序列化设置,默认设置ReferenceLoopHandling.Ignore,Formatting.Indented
        /// </summary>
        public static JsonSerializerSettings DefaultJsonSerializerSettings { get; } = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        /// <summary>
        /// 序列化对象,默认设置ReferenceLoopHandling.Ignore,Formatting.Indented
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonSerializerSettings"></param>
        /// <returns></returns>
        public static string Serialize(object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return JsonConvert.SerializeObject(obj, jsonSerializerSettings ?? DefaultJsonSerializerSettings);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 反序列化为集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static List<T> DeserializeToList<T>(string json)
        {
            return Deserialize<List<T>>(json);
        }

        /// <summary>
        /// 序列化对象到文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <param name="jsonSerializerSettings"></param>
        public static void SerializeToFile(object obj, string filePath, JsonSerializerSettings jsonSerializerSettings = null)
        {
            string s = Serialize(obj, jsonSerializerSettings);
            File.WriteAllText(filePath, s, Encoding.UTF8);
        }

        /// <summary>
        /// 读取文件内容,并反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(string filePath)
        {
            string s = File.ReadAllText(filePath, Encoding.UTF8);

            return Deserialize<T>(s);
        }

        /// <summary>
        /// 读取文件内容,并反序列化为对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<T> DeserializeFromFileToList<T>(string filePath)
        {
            string s = File.ReadAllText(filePath, Encoding.UTF8);

            return DeserializeToList<T>(s);
        }
    }
}
