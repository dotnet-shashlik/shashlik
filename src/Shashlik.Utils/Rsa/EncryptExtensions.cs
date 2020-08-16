using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shashlik.Utils.Rsa
{
    public static class EncryptExtensions
    {
        static readonly Dictionary<RSAEncryptionPadding, int> PaddingLimitDic = new Dictionary<RSAEncryptionPadding, int>()
        {
            [RSAEncryptionPadding.Pkcs1] = 11,
            [RSAEncryptionPadding.OaepSHA1] = 42,
            [RSAEncryptionPadding.OaepSHA256] = 66,
            [RSAEncryptionPadding.OaepSHA384] = 98,
            [RSAEncryptionPadding.OaepSHA512] = 130,
        };

        /// <summary>
        /// 加密,没有使用任何连接 64个字节长度的拼接加密
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr"></param>
        /// <param name="padding"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptBigData(this RSA rsa, string dataStr, RSAEncryptionPadding padding, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var data = encoding.GetBytes(dataStr);
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < data.Length; i += 64)
            {
                var end = i + 64;
                if (end > data.Length)
                    end = data.Length;
                var output = rsa.Encrypt(data[i..end], padding);
                bytes.AddRange(output);
            }

            return Convert.ToBase64String(bytes.ToArray());
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr"></param>
        /// <param name="padding"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptBigData(this RSA rsa, string dataStr, RSAEncryptionPadding padding, Encoding encoding = null)
        {
            var data = Convert.FromBase64String(dataStr);
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < data.Length; i += 128)
            {
                var end = i + 128;
                if (end > data.Length)
                    end = data.Length;
                byte[] output = rsa.Decrypt(data[i..end], padding);
                bytes.AddRange(output);
            }
            return encoding.GetString(bytes.ToArray());
        }
    }
}