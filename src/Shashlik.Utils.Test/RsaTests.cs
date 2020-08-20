using SecurityProxyClient;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class RsaTests
    {
        static RsaTests()
        {
            publicKeyCer = File.ReadAllText(@"D:\testrsa\x509_public.cer");
            publicKeyPem = File.ReadAllText(@"D:\testrsa\rsa_publick_key.pem");
            privateKeyPkcs8 = File.ReadAllText(@"D:\testrsa\pkcs8_rsa_private_key.pem");
            privateKeyPkcs1 = File.ReadAllText(@"D:\testrsa\rsa_private_key.pem");
        }

        static string publicKeyCer { get; }

        static string publicKeyPem { get; }

        static string privateKeyPkcs8 { get; }
        static string privateKeyPkcs1 { get; }

        [Fact]
        public void test()
        {
            string data = "jb3rhg03pj5yl4756860h-590hu4-9hu90-hno[3h945u-5468upmobvg37546-[8pu5m7p[68ih90";

            {
                var a1 = RsaHelper.EncryptByX509(data, publicKeyCer, Encoding.UTF8, RSAEncryptionPadding.Pkcs1);
                var a2 = RsaHelper.Encrypt(data, publicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, RSAEncryptionPadding.Pkcs1, true);

                var d1 = RsaHelper.Decrypt(a1, privateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, RSAEncryptionPadding.Pkcs1, true);
                var d2 = RsaHelper.Decrypt(a2, privateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, RSAEncryptionPadding.Pkcs1, true);
                var d3 = RsaHelper.Decrypt(a1, privateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8, RSAEncryptionPadding.Pkcs1, true);

                d1.ShouldBe(d2);
                d1.ShouldBe(d3);

                var signture = RsaHelper.Sign(data, privateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
                RsaHelper.VerifyByX509(data, signture, publicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signture, publicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
                    .ShouldBe(true);

                var signture1 = RsaHelper.Sign(data, privateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
                RsaHelper.VerifyByX509(data, signture1, publicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signture1, publicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8, HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
                    .ShouldBe(true);
            }

            {
                var a1 = RsaHelper.EncryptByX509(data, publicKeyCer);
                var a2 = RsaHelper.Encrypt(data, publicKeyPem);

                var d1 = RsaHelper.Decrypt(a1, privateKeyPkcs8);
                var d2 = RsaHelper.Decrypt(a2, privateKeyPkcs8);

                d1.ShouldBe(d2);

                var signture = RsaHelper.Sign(data, privateKeyPkcs8);
                RsaHelper.VerifyByX509(data, signture, publicKeyCer)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signture, publicKeyPem)
                    .ShouldBe(true);
            }
        }
    }
}
