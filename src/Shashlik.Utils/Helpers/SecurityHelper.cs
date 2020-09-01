using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

namespace Shashlik.Utils.Helpers
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
            if (value == null) throw new ArgumentNullException(nameof(value));

            encoding ??= Encoding.UTF8;
            using var md5 = MD5.Create();
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
            encoding ??= Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            using var hmacSha256 = new HMACSHA256(keyByte);
            return HashAlgorithmBase(hmacSha256, message, encoding);
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
            encoding ??= Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacSha256 = new HMACSHA256(keyByte);
            byte[] hashMessage = hmacSha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashMessage);
        }

        /// <summary>
        /// HMACSHA1,默认 UTF8编码,原始字符串结果
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA1(string message, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            using var hmacSha1 = new HMACSHA1(keyByte);
            return HashAlgorithmBase(hmacSha1, message, encoding);
        }

        /// <summary>
        /// HMACSHA1,默认 UTF8编码,结果是base64后的,不同于直接HMACSHA1再转base64
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA1Base64(string message, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using var hmacSha1 = new HMACSHA1(keyByte);
            byte[] hashMessage = hmacSha1.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashMessage);
        }

        /// <summary>
        /// SHA1 
        /// </summary>
        public static string Sha1(this string value)
        {
            var encoding = Encoding.UTF8;
            using SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 
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
            foreach (byte btStr in source)
            {
                pwd.AppendFormat(formatStr, btStr);
            }

            return pwd.ToString();
        }

        /// <summary>
        /// HashAlgorithm 加密散列方法
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