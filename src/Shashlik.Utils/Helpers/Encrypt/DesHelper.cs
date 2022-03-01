using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable CheckNamespace

namespace Shashlik.Utils.Helpers
{
    public static class DesHelper
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="key">密钥（必须8位字节码）所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="iv">向量（必须8位字节码）所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>加密后的BASE64</returns>
        public static string Encrypt(string text, string key, string? iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = string.IsNullOrWhiteSpace(iv) ? null : Encoding.UTF8.GetBytes(iv);
            if (keyBytes.Length != 8)
            {
                throw new ArgumentException(nameof(key));
            }

            if (ivBytes is not null && ivBytes.Length != 8)
            {
                throw new ArgumentException(nameof(key));
            }

            using var des = DES.Create();
            des.Padding = paddingMode;
            des.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var cStream = new CryptoStream(ms,
                des.CreateEncryptor(keyBytes, ivBytes),
                CryptoStreamMode.Write);
            var toEncrypt = encoding.GetBytes(text);
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="text">BASE64密文</param>
        /// <param name="key">密钥（必须8位字节码）所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="iv">向量（必须8位字节码）所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>原文</returns>
        public static string Decrypt(string text, string key, string? iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = string.IsNullOrWhiteSpace(iv) ? null : Encoding.UTF8.GetBytes(iv);
            if (keyBytes.Length != 8)
            {
                throw new ArgumentException(nameof(key));
            }

            if (ivBytes is not null && ivBytes.Length != 8)
            {
                throw new ArgumentException(nameof(key));
            }

            var bytes = Convert.FromBase64String(text);
            using var des = DES.Create();
            des.Padding = paddingMode;
            des.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var csDecrypt = new CryptoStream(ms,
                des.CreateDecryptor(keyBytes, ivBytes),
                CryptoStreamMode.Write);

            csDecrypt.Write(bytes, 0, bytes.Length);
            csDecrypt.FlushFinalBlock();
            //Convert the buffer into a string and return it.
            return encoding.GetString(ms.ToArray());
        }
    }
}