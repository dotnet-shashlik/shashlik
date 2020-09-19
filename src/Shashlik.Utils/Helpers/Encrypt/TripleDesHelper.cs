using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Helpers.Encrypt
{
    /// <summary>
    /// 3desc
    /// </summary>
    public class TripleDesHelper
    {
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