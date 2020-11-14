using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RSAExtensions;

namespace Shashlik.Utils.Helpers
{
    /**
     *     # openssl 生成RSA/X509等命令
     * 
     *     # 生成私钥(pkcs1)和公钥(x509)
     *     openssl req -newkey rsa:2048 -nodes -keyout private_pkcs1.pem -x509 -days 3650 -out public.cer
     * 
     *     # 私钥和证书合成PFX加密证书
     *     openssl pkcs12 -export -in public.cer -inkey private_pkcs1.pem -out test.pfx
     *
     *     # pkcs1私钥转pkcs8
     *     openssl pkcs8 -topk8 -in private_pkcs1.pem -out private_pkcs8.pem -nocrypt
     *
     *     # 私钥导出x509公钥证书
     *     openssl req -new -x509 -key private_pkcs1.pem -out public.cer -days 365
     *
     *     # 私钥导出pem公钥
     *     openssl rsa -in private_pkcs1.pem -pubout -out public.pem
     */
    public static class RsaHelper
    {
        static readonly Dictionary<RSAEncryptionPadding, int> PaddingLimitDic =
            new Dictionary<RSAEncryptionPadding, int>()
            {
                [RSAEncryptionPadding.Pkcs1] = 11,
                [RSAEncryptionPadding.OaepSHA1] = 42,
                [RSAEncryptionPadding.OaepSHA256] = 66,
                [RSAEncryptionPadding.OaepSHA384] = 98,
                [RSAEncryptionPadding.OaepSHA512] = 130,
            };

        /// <summary>
        /// 从私钥构建RSA对象
        /// </summary>
        /// <param name="privateKey">私钥内容</param>
        /// <param name="keyType">私钥类型</param>
        /// <param name="isPem">是否为pem格式</param>
        /// <returns></returns>
        public static RSA FromPrivateKey(string privateKey, RSAKeyType keyType, bool isPem)
        {
            var rsa = RSA.Create();
            rsa.ImportPrivateKey(keyType, privateKey, isPem);
            return rsa;
        }

        /// <summary>
        /// 从公钥构建RSA对象
        /// </summary>
        /// <param name="publicKey">公钥内容</param>
        /// <param name="keyType">公钥类型</param>
        /// <param name="isPem">是否为pem格式</param>
        /// <returns></returns>
        public static RSA FromPublicKey(string publicKey, RSAKeyType keyType, bool isPem)
        {
            var rsa = RSA.Create();
            rsa.ImportPublicKey(keyType, publicKey, isPem);
            return rsa;
        }

        /// <summary>
        /// 从base64编码后的证书文件加载X509证书
        /// </summary>
        /// <param name="certificateBase64Content">证书内容</param>
        /// <param name="password">证书密码</param>
        /// <returns></returns>
        public static X509Certificate2 LoadX509FromBase64(string certificateBase64Content, string password = null)
        {
            return password is null
                ? new X509Certificate2(Convert.FromBase64String(certificateBase64Content))
                : new X509Certificate2(Convert.FromBase64String(certificateBase64Content), password);
        }

        /// <summary>
        /// 加载x509的公钥证书,比如cer,crt文件
        /// </summary>
        /// <param name="certificateContent">证书内容</param>
        /// <returns></returns>
        public static X509Certificate2 LoadX509FromPublicCertificate(string certificateContent)
        {
            return new X509Certificate2(Encoding.UTF8.GetBytes(certificateContent));
        }

        /// <summary>
        /// RSA公钥加密,未分块
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="data">待加密数据</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(this RSA rsa, string data, RSAEncryptionPadding padding, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var dataBytes = encoding.GetBytes(data);
            var resBytes = rsa.Encrypt(dataBytes, padding);
            return Convert.ToBase64String(resBytes);
        }

        /// <summary>
        /// 公钥大数据分块加密,使用<paramref name="connChar"/>作为块数据连接
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr">待加密字符串</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding">明文编码方式,默认utf8</param>
        /// <param name="connChar">块数据连接字符</param>
        /// <returns></returns>
        public static string EncryptBigDataWithSplit(this RSA rsa, string dataStr, RSAEncryptionPadding padding,
            Encoding encoding = null, char? connChar = '$')
        {
            if (!connChar.HasValue)
                return EncryptBigData(rsa, dataStr, padding, encoding);

            if (connChar <= 0) throw new ArgumentOutOfRangeException(nameof(connChar));
            encoding ??= Encoding.UTF8;
            var data = encoding.GetBytes(dataStr);
            var modulusLength = rsa.KeySize / 8;
            var splitLength = modulusLength - PaddingLimitDic[padding];
            var sb = new StringBuilder();
            var splitsNumber = Convert.ToInt32(Math.Ceiling(data.Length * 1.0 / splitLength));

            var pointer = 0;
            for (var i = 0; i < splitsNumber; i++)
            {
                var current = pointer + splitLength < data.Length
                    ? data.Skip(pointer).Take(splitLength).ToArray()
                    : data.Skip(pointer).Take(data.Length - pointer).ToArray();

                sb.Append(Convert.ToBase64String(rsa.Encrypt(current, padding)));
                sb.Append(connChar);
                pointer += splitLength;
            }

            return sb.ToString().TrimEnd(connChar.Value);
        }

        /// <summary>
        /// RSA 无长度限制加密, 无分块连接字符
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr">需要加密的数据</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptBigData(this RSA rsa, string dataStr, RSAEncryptionPadding padding,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            if (string.IsNullOrEmpty(dataStr)) return string.Empty;
            if (rsa is null) throw new ArgumentException("public key can not null");
            var inputBytes = encoding.GetBytes(dataStr);
            var bufferSize = (rsa.KeySize / 8) - 11;
            var buffer = new byte[bufferSize];
            using MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream();
            while (true)
            {
                var readSize = inputStream.Read(buffer, 0, bufferSize);
                if (readSize <= 0) break;
                var temp = new byte[readSize];
                Array.Copy(buffer, 0, temp, 0, readSize);
                var encryptedBytes = rsa.Encrypt(temp, padding);
                outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            }

            return Convert.ToBase64String(outputStream.ToArray());
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="data">待解密数据</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(this RSA rsa, string data, RSAEncryptionPadding padding, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            if (rsa is null) throw new ArgumentException("private key can not null");

            var dataBytes = Convert.FromBase64String(data);
            var resBytes = rsa.Decrypt(dataBytes, padding);
            return encoding.GetString(resBytes);
        }

        /// <summary>
        /// RSA 大数据分块解密,使用<paramref name="connChar"/>分割
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr">待解密数据</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"></param>
        /// <param name="connChar">块数据分割符</param>
        /// <returns></returns>
        public static string DecryptBigDataWithSplit(this RSA rsa, string dataStr, RSAEncryptionPadding padding,
            Encoding encoding = null, char connChar = '$')
        {
            encoding ??= Encoding.UTF8;
            if (rsa is null)
            {
                throw new ArgumentException("private key can not null");
            }

            var data = dataStr.Split(new[] {connChar}, StringSplitOptions.RemoveEmptyEntries);
            var byteList = new List<byte>();

            foreach (var item in data)
            {
                byteList.AddRange(rsa.Decrypt(Convert.FromBase64String(item), padding));
            }

            return encoding.GetString(byteList.ToArray());
        }

        /// <summary>
        /// RSA 大数据解密,无分割字符
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="dataStr">需要解密的数据</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptBigData(this RSA rsa, string dataStr, RSAEncryptionPadding padding,
            Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            if (string.IsNullOrEmpty(dataStr)) return string.Empty;
            if (rsa is null) throw new ArgumentException("private key can not null");
            var inputBytes = Convert.FromBase64String(dataStr);
            var bufferSize = rsa.KeySize / 8;
            var buffer = new byte[bufferSize];
            using MemoryStream inputStream = new MemoryStream(inputBytes), outputStream = new MemoryStream();
            while (true)
            {
                var readSize = inputStream.Read(buffer, 0, bufferSize);
                if (readSize <= 0) break;
                var temp = new byte[readSize];
                Array.Copy(buffer, 0, temp, 0, readSize);
                var rawBytes = rsa.Decrypt(temp, padding);
                outputStream.Write(rawBytes, 0, rawBytes.Length);
            }

            return encoding.GetString(outputStream.ToArray());
        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="data">待签名数据</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"><paramref name="data"/>编码格式</param>
        /// <returns>Sign bytes</returns>
        public static string SignData(this RSA rsa, string data, HashAlgorithmName hashAlgorithmName,
            RSASignaturePadding padding, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            var res = SignDataGetBytes(rsa, data, hashAlgorithmName, padding, encoding);
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="data">待签名数据</param>
        /// <param name="hashAlgorithmName">签名算法</param>
        /// <param name="padding">填充算法</param>
        /// <param name="encoding"><paramref name="data"/>编码格式</param>
        /// <returns>Sign bytes</returns>
        public static byte[] SignDataGetBytes(this RSA rsa, string data, HashAlgorithmName hashAlgorithmName,
            RSASignaturePadding padding, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            if (rsa is null)
            {
                throw new ArgumentException("private key can not null");
            }

            var dataBytes = encoding.GetBytes(data);
            return rsa.SignData(dataBytes, hashAlgorithmName, padding);
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="data">待验证数据</param>
        /// <param name="sign">签名值</param>
        /// <param name="hashAlgorithmName">签名算法是</param>
        /// <param name="padding">填充算法是</param>
        /// <param name="encoding"><paramref name="data"></paramref>编码格式</param>
        /// <returns></returns>
        public static bool VerifySignData(this RSA rsa, string data, string sign, HashAlgorithmName hashAlgorithmName,
            RSASignaturePadding padding, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            if (rsa is null)
            {
                throw new ArgumentException("public key can not null");
            }

            var dataBytes = encoding.GetBytes(data);
            var signBytes = Convert.FromBase64String(sign);
            var res = rsa.VerifyData(dataBytes, signBytes, hashAlgorithmName, padding);
            return res;
        }
    }
}