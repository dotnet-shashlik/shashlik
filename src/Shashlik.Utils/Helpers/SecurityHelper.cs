using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Extensions
{
    /// <summary>
    /// 加解密
    /// </summary>
    public static class SecurityHelper
    {

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string Md532(string value, Encoding encoding = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            encoding = encoding ?? Encoding.UTF8;
            using MD5 md5 = MD5.Create();
            return HashAlgorithmBase(md5, value, encoding);
        }

        /// <summary>
        /// HMACSHA256,默认 UTF8编码,原始字符串结果
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA256(string message, string secret, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            using var hmacsha256 = new HMACSHA256(keyByte);
            return HashAlgorithmBase(hmacsha256, message, encoding);
        }

        /// <summary>
        /// HMACSHA256,默认 UTF8编码,结果是base64后的,不同于直接HMACSHA256再转base64
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA256Base64(string message, string secret, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacsha256 = new HMACSHA256(keyByte);
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashmessage);
        }

        /// <summary>
        /// SHA1 加密
        /// </summary>
        public static string Sha1(this string value)
        {
            var encoding = Encoding.UTF8;
            using SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 加密
        /// </summary>
        public static string Sha256(this string value)
        {
            var encoding = Encoding.UTF8;
            using SHA256 sha256 = new SHA256Managed();
            return HashAlgorithmBase(sha256, value, encoding);
        }

        /// <summary>
        /// SHA512 加密
        /// </summary>
        public static string Sha512(this string value)
        {
            var encoding = Encoding.UTF8;
            using SHA512 sha512 = new SHA512Managed();
            return HashAlgorithmBase(sha512, value, encoding);
        }

        #region private

        /// <summary>
        /// 转换成字符串
        /// </summary>
        static string Bytes2Str(this IEnumerable<byte> source, string formatStr = "{0:x2}")
        {
            StringBuilder pwd = new StringBuilder();
            foreach (byte btStr in source) { pwd.AppendFormat(formatStr, btStr); }
            return pwd.ToString();
        }

        /// <summary>
        /// HashAlgorithm 加密统一方法
        /// </summary>
        static string HashAlgorithmBase(HashAlgorithm hashAlgorithmObj, string source, Encoding encoding)
        {
            byte[] btStr = encoding.GetBytes(source);
            byte[] hashStr = hashAlgorithmObj.ComputeHash(btStr);
            return hashStr.Bytes2Str();
        }

        #endregion
    }
}
