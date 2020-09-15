using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shashlik.Utils.Helpers.Encrypt
{
    public static class HashHelper
    {
        #region string hash

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
        /// HmacSha256,默认 UTF8编码,原始字符串结果
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HmacSha256(string value, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            using var hmacSha256 = new HMACSHA256(keyByte);
            return HashAlgorithmBase(hmacSha256, value, encoding);
        }

        /// <summary>
        /// HmacSha256,默认 UTF8编码,结果是base64后的,不同于直接不同于直接HmacSha256再转base64再转base64
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HmacSha256Base64(string value, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            var valueBytes = encoding.GetBytes(value);
            using var hmacSha256 = new HMACSHA256(keyByte);
            var hashValue = hmacSha256.ComputeHash(valueBytes);
            return Convert.ToBase64String(hashValue);
        }

        /// <summary>
        /// HmacSha1,默认 UTF8编码,原始字符串结果
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HmacSha1(string value, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            using var hmacSha1 = new HMACSHA1(keyByte);
            return HashAlgorithmBase(hmacSha1, value, encoding);
        }

        /// <summary>
        /// HmacSha1,默认 UTF8编码,结果是base64后的,不同于直接HmacSha1再转base64
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HmacSha1Base64(string value, string secret, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            var valueBytes = encoding.GetBytes(value);
            using var hmacSha1 = new HMACSHA1(keyByte);
            var hashvalue = hmacSha1.ComputeHash(valueBytes);
            return Convert.ToBase64String(hashvalue);
        }

        /// <summary>
        /// SHA1 
        /// </summary>
        public static string Sha1(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 
        /// </summary>
        public static string Sha256(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA256 sha256 = new SHA256Managed();
            return HashAlgorithmBase(sha256, value, encoding);
        }

        /// <summary>
        /// SHA512 加密
        /// </summary>
        public static string Sha512(string value, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA512 sha512 = new SHA512Managed();
            return HashAlgorithmBase(sha512, value, encoding);
        }

        #region private

        /// <summary>
        /// 转换成字符串
        /// </summary>
        private static string Bytes2Str(this IEnumerable<byte> source, string formatStr = "{0:x2}")
        {
            var pwd = new StringBuilder();
            foreach (var btStr in source)
            {
                pwd.AppendFormat(formatStr, btStr);
            }

            return pwd.ToString();
        }

        /// <summary>
        /// HashAlgorithm 加密散列方法
        /// </summary>
        private static string HashAlgorithmBase(HashAlgorithm hashAlgorithmObj, string source, Encoding encoding)
        {
            var btStr = encoding.GetBytes(source);
            var hashStr = hashAlgorithmObj.ComputeHash(btStr);
            return hashStr.Bytes2Str();
        }

        #endregion

        #endregion
    }
}