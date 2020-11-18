using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// ReSharper disable CheckNamespace

namespace Shashlik.Utils.Helpers
{
    public static class AesHelper
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="key">密钥（必须16位字节码）,所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="iv">向量（必须16位字节码）,所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>加密后的BASE64</returns>
        public static string Encrypt(string text, string key, string iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            if (keyBytes.Length != 16)
            {
                throw new ArgumentException(nameof(key));
            }

            if (ivBytes.Length != 16)
            {
                throw new ArgumentException(nameof(key));
            }

            using var aes = Aes.Create();
            Debug.Assert(aes != null, nameof(aes) + " != null");
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Padding = paddingMode;
            aes.Mode = cipherMode;
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var csEncrypt = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            var bytes = encoding.GetBytes(text);
            csEncrypt.Write(bytes, 0, bytes.Length);
            csEncrypt.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text">BASE64密文</param>
        /// <param name="key">密钥（必须16位字节码）,所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="iv">向量（必须16位字节码）,所以最好不要使用多字节的字符,比如中文等等</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>原文</returns>
        public static string Decrypt(string text, string key, string iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);
            if (keyBytes.Length != 16)
            {
                throw new ArgumentException(nameof(key));
            }

            if (ivBytes.Length != 16)
            {
                throw new ArgumentException(nameof(key));
            }

            var bytes = Convert.FromBase64String(text);
            using var aes = Aes.Create();
            aes.Padding = paddingMode;
            aes.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var csDecrypt = new CryptoStream(ms,
                aes.CreateDecryptor(keyBytes, ivBytes),
                CryptoStreamMode.Write);

            csDecrypt.Write(bytes, 0, bytes.Length);
            csDecrypt.FlushFinalBlock();
            //Convert the buffer into a string and return it.
            return encoding.GetString(ms.ToArray());
        }
    }
}