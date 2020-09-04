using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shashlik.Utils.Encrypt
{
    public static class AesExtensions
    {
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="key">密钥（必须16位）</param>
        /// <param name="iv">向量（必须16位）</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>加密后的BASE64</returns>
        public static string AesEncrypt(this string text, byte[] key, byte[] iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
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
        /// <param name="key">密钥（必须16位）</param>
        /// <param name="iv">向量（必须16位）</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>原文</returns>
        public static string AesDecrypt(this string text, byte[] key, byte[] iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = Convert.FromBase64String(text);
            var aes = Aes.Create();
            aes.Padding = paddingMode;
            aes.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var csDecrypt = new CryptoStream(ms,
                aes.CreateDecryptor(key, iv),
                CryptoStreamMode.Write);
            
            csDecrypt.Write(bytes, 0, bytes.Length);
            csDecrypt.FlushFinalBlock();
            //Convert the buffer into a string and return it.
            return encoding.GetString(ms.ToArray());
        }
    }
}