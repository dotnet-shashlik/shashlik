using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;
// ReSharper disable CheckNamespace

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 3des
    /// </summary>
    public class TripleDesHelper
    {
        /// <summary>
        ///  加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密模式</param>
        /// <param name="paddingMode">填充模式</param>
        /// <param name="encoder">明文编码方式</param>
        /// <param name="iv">加密向量</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string key, CipherMode mode = CipherMode.ECB,
            PaddingMode paddingMode = PaddingMode.PKCS7, Encoding encoder = null, string iv = null)
        {
            encoder ??= Encoding.UTF8;
            using var des = CreateDes(key);
            using var ct = des.CreateEncryptor();
            var input = encoder.GetBytes(plainText);
            var output = ct.TransformFinalBlock(input, 0, input.Length);
            return Convert.ToBase64String(output);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cypherText">密文</param>
        /// <param name="key">密钥</param>
        /// <param name="mode">加密模式</param>
        /// <param name="paddingMode">填充模式</param>
        /// <param name="encoder">明文编码方式</param>
        /// <param name="iv">加密向量</param>
        /// <returns></returns>
        public static string Decrypt(string cypherText, string key, CipherMode mode = CipherMode.ECB,
            PaddingMode paddingMode = PaddingMode.PKCS7, Encoding encoder = null,
            string iv = null)
        {
            encoder ??= Encoding.UTF8;
            using var des = CreateDes(key);
            using var ct = des.CreateDecryptor();
            var input = Convert.FromBase64String(cypherText);
            var output = ct.TransformFinalBlock(input, 0, input.Length);
            return encoder.GetString(output);
        }

        private static TripleDES CreateDes(string key, CipherMode mode = CipherMode.ECB,
            PaddingMode paddingMode = PaddingMode.PKCS7, string iv = null)
        {
            TripleDES des = new TripleDESCryptoServiceProvider();
            var desKey = Encoding.ASCII.GetBytes(key);
            des.Key = desKey;
            if (!iv.IsNullOrWhiteSpace())
                des.IV = Encoding.ASCII.GetBytes(iv);
            des.Padding = paddingMode;
            des.Mode = mode;
            return des;
        }
    }
}