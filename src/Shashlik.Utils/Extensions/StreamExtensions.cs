using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Utils.Extensions
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
            try
            {
                using (System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                    return sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5Hash() fail, error:" + ex.Message);
            }
        }

        public static string ReadToString(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                if (stream.CanSeek && stream.Position != 0)
                    stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(ms);
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                var data = ms.ToArray();
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
        }

        public static string ReadToString(this Stream stream, Encoding encoding)
        {
            using (var ms = new MemoryStream())
            {
                if (stream.CanSeek && stream.Position != 0)
                    stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(ms);
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
                var data = ms.ToArray();
                return encoding.GetString(data, 0, data.Length);
            }
        }

        public static byte[] ReadAll(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
