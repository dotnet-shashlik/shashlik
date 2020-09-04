﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Shashlik.Utils.Encrypt
{
    public static class DesExtensions
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="text">原文</param>
        /// <param name="key">密钥（必须8位）</param>
        /// <param name="iv">向量（必须8位）</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>加密后的BASE64</returns>
        public static string DesEncrypt(this string text, byte[] key, byte[] iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var des = DES.Create();
            des.Padding = paddingMode;
            des.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var cStream = new CryptoStream(ms,
                des.CreateEncryptor(key, iv),
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
        /// <param name="key">密钥（必须8位）</param>
        /// <param name="iv">向量（必须8位）</param>
        /// <param name="paddingMode">PaddingMode默认PKCS7</param>
        /// <param name="cipherMode">CipherMode默认CBC</param>
        /// <param name="encoding">原文编码默认utf8</param>
        /// <returns>原文</returns>
        public static string DesDecrypt(this string text, byte[] key, byte[] iv,
            PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var bytes = Convert.FromBase64String(text);
            var des = DES.Create();
            des.Padding = paddingMode;
            des.Mode = cipherMode;
            using var ms = new MemoryStream();
            using var csDecrypt = new CryptoStream(ms,
                des.CreateDecryptor(key, iv),
                CryptoStreamMode.Write);
            
            csDecrypt.Write(bytes, 0, bytes.Length);
            csDecrypt.FlushFinalBlock();
            //Convert the buffer into a string and return it.
            return encoding.GetString(ms.ToArray());
        }
    }
}