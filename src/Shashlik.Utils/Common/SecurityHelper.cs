using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Guc.Utils.Extensions;

namespace Guc.Utils.Extensions
{
    /// <summary>
    /// 加解密
    /// </summary>
    public static class SecurityHelper
    {
        #region MD5加密

        /// <summary>
        /// MD5加密
        /// </summary>
        public static string Md532(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            MD5 md5 = MD5.Create();
            return HashAlgorithmBase(md5, value, encoding);
        }

        #endregion

        #region SHA 加密

        /// <summary>
        /// HMACSHA256,默认 UTF8编码,结果是base64后的
        /// </summary>
        /// <param name="message"></param>
        /// <param name="secret"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string HMACSHA256(string message, string secret, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        /// <summary>
        /// SHA1 加密
        /// </summary>
        public static string Sha1(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            return HashAlgorithmBase(sha1, value, encoding);
        }

        /// <summary>
        /// SHA256 加密
        /// </summary>
        public static string Sha256(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }

            var encoding = Encoding.UTF8;
            SHA256 sha256 = new SHA256Managed();
            return HashAlgorithmBase(sha256, value, encoding);
        }

        /// <summary>
        /// SHA512 加密
        /// </summary>
        public static string Sha512(this string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("未将对象引用设置到对象的实例。");
            }
            var encoding = Encoding.UTF8;
            SHA512 sha512 = new SHA512Managed();
            return HashAlgorithmBase(sha512, value, encoding);
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 转换成字符串
        /// </summary>
        private static string Bytes2Str(this IEnumerable<byte> source, string formatStr = "{0:X2}")
        {
            StringBuilder pwd = new StringBuilder();
            foreach (byte btStr in source) { pwd.AppendFormat(formatStr, btStr); }
            return pwd.ToString();
        }

        /// <summary>
        /// HashAlgorithm 加密统一方法
        /// </summary>
        private static string HashAlgorithmBase(HashAlgorithm hashAlgorithmObj, string source, Encoding encoding)
        {
            byte[] btStr = encoding.GetBytes(source);
            byte[] hashStr = hashAlgorithmObj.ComputeHash(btStr);
            return hashStr.Bytes2Str();
        }

        #endregion
    }
}
