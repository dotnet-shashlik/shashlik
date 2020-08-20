using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Rsa;

namespace SecurityProxyClient
{
    /// <summary>
    /// RsaHelper,加密/解密/签名/验签
    /// </summary>
    public class RsaHelper
    {
        /// <summary>
        /// 公钥加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">明文编码方式</param>
        /// <param name="padding">填充方式</param>
        /// <returns></returns>
        public static string Encrypt(string data, string publicKey, Encoding encoding, RSAEncryptionPadding padding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
            {
                return ((RSA)cer.PublicKey.Key).EncryptBigData(data, padding, encoding);
            }
        }

        /// <summary>
        /// 私钥解密,pkcs8
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <param name="privateKey">私钥:pem</param>
        /// <param name="encoding">解密密文编码</param>
        /// <param name="padding">填充方式</param>
        /// <param name="keySize">密钥长度</param>
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey, Encoding encoding, RSAEncryptionPadding padding, int keySize = 1024)
        {
            using (var rsa = RSA.Create(keySize))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                return rsa.DecryptBigData(data, padding, encoding);
            }
        }

        /// <summary>
        /// 私钥签名,pkcs8
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="privateKey">私钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <param name="hash">签名hash算法</param>
        /// <param name="padding">签名填充模式,pss/pkcs1,pss安全性更好</param>
        /// <param name="keySize">密钥长度</param>
        /// <returns>签名</returns>
        public static string Sign(string data, string privateKey, Encoding encoding, HashAlgorithmName hash, RSASignaturePadding padding, int keySize = 1024)
        {
            using (var rsa = RSA.Create(keySize))
            {
                rsa.ImportPrivateKey(RSAKeyType.Pkcs8, privateKey, true);
                var signed = rsa.SignData(encoding.GetBytes(data), hash, padding);
                return Convert.ToBase64String(signed);
            }
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <param name="hash">签名hash算法</param>
        /// <param name="padding">签名填充模式</param>
        /// <returns></returns>
        public static bool Verify(string data, string signature, string publicKey, Encoding encoding, HashAlgorithmName hash, RSASignaturePadding padding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
                return ((RSA)cer.PublicKey.Key).VerifyData(
                    encoding.GetBytes(data),
                    Convert.FromBase64String(signature),
                    hash,
                    padding
                    );

        }
    }
}
