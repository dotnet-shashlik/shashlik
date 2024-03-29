﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Shashlik.Utils.Helpers
{
    public static class HashHelper
    {
        #region string hash

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string MD5(string value, Encoding? encoding = null)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));

            encoding ??= Encoding.UTF8;
            using var md5 = System.Security.Cryptography.MD5.Create();
            return HashAlgorithmBase(md5, value, encoding);
        }

        /// <summary>
        /// HmacSha256,默认 UTF8编码,结果是base64后的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA256(string value, string secret, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            var valueBytes = encoding.GetBytes(value);
            using var hmacSha256 = new HMACSHA256(keyByte);
            var hashValue = hmacSha256.ComputeHash(valueBytes);
            return Convert.ToBase64String(hashValue);
        }

        /// <summary>
        /// HmacSha1,默认 UTF8编码,结果是base64后的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA1(string value, string secret, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyByte = encoding.GetBytes(secret);
            var valueBytes = encoding.GetBytes(value);
            using var hmacSha1 = new HMACSHA1(keyByte);
            var hashValue = hmacSha1.ComputeHash(valueBytes);
            return Convert.ToBase64String(hashValue);
        }

        /// <summary>
        /// SHA1 
        /// </summary>
        public static string SHA1(string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 
        /// </summary>
        public static string SHA256(string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            return HashAlgorithmBase(sha256, value, encoding);
        }

        /// <summary>
        /// SHA512
        /// </summary>
        public static string SHA512(string value, Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using SHA512 sha512 = System.Security.Cryptography.SHA512.Create();
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