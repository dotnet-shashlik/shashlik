using System.IO;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Shashlik.Utils.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// 获取数据流的MD5值
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetMD5Hash(this Stream stream)
        {
            using System.Security.Cryptography.MD5
                md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(stream);
            var sb = new StringBuilder();
            foreach (var t in retVal)
                sb.Append(t.ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// 读取到字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string ReadToString(this Stream stream, Encoding? encoding = null)
        {
            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);

            encoding ??= Encoding.UTF8;

            using var streamReader = new StreamReader(stream, encoding, true, 1024, true);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// 读取到字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static async Task<string> ReadToStringAsync(this Stream stream, Encoding? encoding = null)
        {
            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);

            encoding ??= Encoding.UTF8;

            using var streamReader = new StreamReader(stream, encoding, true, 1024, true);
            return await streamReader.ReadToEndAsync();
        }

        /// <summary>
        /// 读取所有的字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadAll(this Stream stream)
        {
            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }

        /// <summary>
        /// 读取所有的字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllAsync(this Stream stream)
        {
            if (stream.CanSeek && stream.Position != 0)
                stream.Seek(0, SeekOrigin.Begin);
            var data = new byte[stream.Length];
            await stream.ReadAsync(data, 0, data.Length);
            return data;
        }
    }
}