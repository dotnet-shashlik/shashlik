using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Shashlik.Utils.Rsa;

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// RsaHelper,加密/解密/签名/验签
    /// </summary>
    public class RsaHelper
    {
        /// <summary>
        /// x509公钥加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">x509公钥</param>
        /// <param name="encoding">明文编码方式</param>
        /// <param name="padding">填充方式</param>
        /// <returns></returns>
        public static string EncryptByX509(string data, string publicKey, Encoding encoding, RSAEncryptionPadding padding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
            {
                return ((RSA)cer.PublicKey.Key).EncryptBigData(data, padding, encoding);
            }
        }

        /// <summary>
        /// x509公钥加密,填充模式pkcs1
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">x509公钥</param>
        /// <param name="encoding">明文编码方式</param>-
        /// <returns></returns>
        public static string EncryptByX509(string data, string publicKey, string encoding = "UTF-8")
        {
            return EncryptByX509(data, publicKey, Encoding.GetEncoding(encoding), RSAEncryptionPadding.Pkcs1);
        }

        /// <summary>
        /// rsa公钥加密
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="keyType">公钥格式</param>
        /// <param name="encoding">明文编码方式</param>
        /// <param name="padding">填充方式</param>
        /// <param name="isPem">是否为pem格式</param>/// 
        /// <returns></returns>
        public static string Encrypt(string data, string publicKey, RSAKeyType keyType, Encoding encoding,
            RSAEncryptionPadding padding, bool isPem)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPublicKey(keyType, publicKey, isPem);
                return rsa.EncryptBigData(data, padding, encoding);
            }
        }

        /// <summary>
        /// rsa公钥加密:(pem+pkcs8公钥格式)+pkcs1(填充模式)
        /// </summary>
        /// <param name="data">明文</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">明文编码方式</param>
        /// <returns></returns>
        public static string Encrypt(string data, string publicKey, string encoding = "UTF-8")
        {
            return Encrypt(data, publicKey, RSAKeyType.Pkcs8, Encoding.GetEncoding(encoding), RSAEncryptionPadding.Pkcs1, true);
        }

        /// <summary>
        /// 私钥解密
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <param name="privateKey">pkcs8,私钥:pem</param>
        /// <param name="keyType">私钥格式</param>
        /// <param name="encoding">解密密文编码</param>
        /// <param name="padding">填充方式</param>
        /// <param name="isPem">是否为pem格式</param>/// 
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey, RSAKeyType keyType, Encoding encoding,
            RSAEncryptionPadding padding, bool isPem)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPrivateKey(keyType, privateKey, isPem);
                return rsa.DecryptBigData(data, padding, encoding);
            }
        }

        /// <summary>
        /// 私钥解密:(pem+pkcs8公钥格式)+pkcs1(填充模式)
        /// </summary>
        /// <param name="data">密文数据</param>
        /// <param name="privateKey">pkcs8,私钥:pem</param>
        /// <param name="encoding">解密密文编码</param>
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey, string encoding = "UTF-8")
        {
            return Decrypt(data, privateKey, RSAKeyType.Pkcs8, Encoding.GetEncoding(encoding), RSAEncryptionPadding.Pkcs1, true);
        }

        /// <summary>
        /// 私钥签名
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="privateKey">pkcs8,私钥:pem</param>
        /// <param name="keyType">私钥格式</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <param name="hash">签名hash算法</param>
        /// <param name="padding">签名填充模式,pss/pkcs1,pss安全性更好</param>
        /// <param name="isPem">是否为pem格式</param>/// 
        /// <returns>签名</returns>
        public static string Sign(string data, string privateKey, RSAKeyType keyType, Encoding encoding,
            HashAlgorithmName hash, RSASignaturePadding padding, bool isPem)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPrivateKey(keyType, privateKey, isPem);
                var signed = rsa.SignData(encoding.GetBytes(data), hash, padding);
                return Convert.ToBase64String(signed);
            }
        }

        /// <summary>
        /// 私钥签名:(pem+pkcs8私钥格式)+(sha256散列算法)+(pss填充模式)
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="privateKey">pkcs8,私钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <returns>签名</returns>
        public static string Sign(string data, string privateKey, string encoding = "UTF-8")
        {
            return Sign(data, privateKey, RSAKeyType.Pkcs8, Encoding.GetEncoding(encoding), HashAlgorithmName.SHA256, RSASignaturePadding.Pss, true);
        }

        /// <summary>
        /// x509公钥验签
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <param name="hash">签名hash算法</param>
        /// <param name="padding">签名填充模式</param>
        /// <returns></returns>
        public static bool VerifyByX509(string data, string signature, string publicKey, Encoding encoding, HashAlgorithmName hash, RSASignaturePadding padding)
        {
            using (var cer = new X509Certificate2(Encoding.UTF8.GetBytes(publicKey)))
                return ((RSA)cer.PublicKey.Key).VerifyData(
                    encoding.GetBytes(data),
                    Convert.FromBase64String(signature),
                    hash,
                    padding
                    );

        }

        /// <summary>
        /// x509公钥验签: (sha256散列算法)+(pss填充模式)
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <returns></returns>
        public static bool VerifyByX509(string data, string signature, string publicKey, string encoding = "UTF-8")
        {
            return VerifyByX509(data, signature, publicKey, Encoding.GetEncoding(encoding), HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
        }

        /// <summary>
        /// 公钥验签
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="keyType">公钥格式</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <param name="hash">签名hash算法</param>
        /// <param name="padding">签名填充模式</param>
        /// <param name="isPem">是否为pem格式</param>
        /// <returns></returns>
        public static bool Verify(string data, string signature, string publicKey, RSAKeyType keyType
            , Encoding encoding, HashAlgorithmName hash, RSASignaturePadding padding, bool isPem)
        {
            using (var rsa = RSA.Create())
            {
                rsa.ImportPublicKey(keyType, publicKey, isPem);
                return rsa.VerifyData(
                    encoding.GetBytes(data),
                    Convert.FromBase64String(signature),
                    hash,
                    padding
                    );
            }
        }

        /// <summary>
        /// 公钥验签:(pem+pkcs8私钥格式)+(sha256散列算法)+(pss填充模式)
        /// </summary>
        /// <param name="data">待签名数据</param>
        /// <param name="signature">签名</param>
        /// <param name="publicKey">公钥:pem</param>
        /// <param name="encoding">待签名数据编码方式</param>
        /// <returns></returns>
        public static bool Verify(string data, string signature, string publicKey, string encoding = "UTF-8")
        {
            return Verify(data, signature, publicKey, RSAKeyType.Pkcs8,
                Encoding.GetEncoding(encoding), HashAlgorithmName.SHA256, RSASignaturePadding.Pss, true);
        }
    }
}
