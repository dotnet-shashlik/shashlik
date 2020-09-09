using System;
using Shouldly;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using Shashlik.Utils.Helpers;
using Shashlik.Utils.Rsa;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class RsaTests
    {
        static RsaTests()
        {
            PublicKeyCer = File.ReadAllText(@"./rsatest.demokey/x509_public.cer");
            PublicKeyPem = File.ReadAllText(@"./rsatest.demokey/rsa_public_key.pem");
            PrivateKeyPkcs8 = File.ReadAllText(@"./rsatest.demokey/pkcs8_rsa_private_key.pem");
            PrivateKeyPkcs1 = File.ReadAllText(@"./rsatest.demokey/rsa_private_key.pem");
            var pfxBytes = File.ReadAllBytes(@"./rsatest.demokey/test.pfx");
            PfxBase64 = Convert.ToBase64String(pfxBytes);
        }

        private static string PublicKeyCer { get; }
        private static string PublicKeyPem { get; }
        private static string PrivateKeyPkcs8 { get; }
        private static string PrivateKeyPkcs1 { get; }
        private static string PfxBase64 { get; }

        [Fact]
        public void PfxTest()
        {
            var data = Guid.NewGuid().ToString();
            using var cer = RsaHelper.LoadX509FromBase64(PfxBase64, "123123");
            var encoded = cer.GetRSAPublicKey().EncryptBigData(data, RSAEncryptionPadding.Pkcs1);
            var decoded = cer.GetRSAPrivateKey().DecryptBigData(encoded, RSAEncryptionPadding.Pkcs1);
            decoded.ShouldBe(data);
        }


        [Fact]
        public void IntegrationTest()
        {
            // var data = Guid.NewGuid().ToString();
            //
            // {
            //     var a1 = RsaHelper.EncryptByX509(data, PublicKeyCer, Encoding.UTF8, RSAEncryptionPadding.Pkcs1);
            //     var a2 = RsaHelper.Encrypt(data, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //         RSAEncryptionPadding.Pkcs1, true);
            //
            //     var d1 = RsaHelper.Decrypt(a1, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //         RSAEncryptionPadding.Pkcs1, true);
            //     var d2 = RsaHelper.Decrypt(a2, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //         RSAEncryptionPadding.Pkcs1, true);
            //     var d3 = RsaHelper.Decrypt(a1, PrivateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8,
            //         RSAEncryptionPadding.Pkcs1, true);
            //
            //     d1.ShouldBe(d2);
            //     d1.ShouldBe(d3);
            //
            //     var signature = RsaHelper.Sign(data, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //         HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
            //     RsaHelper.VerifyByX509(data, signature, PublicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5,
            //             RSASignaturePadding.Pss)
            //         .ShouldBe(true);
            //     RsaHelper.Verify(data, signature, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //             HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
            //         .ShouldBe(true);
            //
            //     var signature1 = RsaHelper.Sign(data, PrivateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8,
            //         HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
            //     RsaHelper.VerifyByX509(data, signature1, PublicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5,
            //             RSASignaturePadding.Pss)
            //         .ShouldBe(true);
            //     RsaHelper.Verify(data, signature1, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
            //             HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
            //         .ShouldBe(true);
            // }
            //
            // {
            //     var a1 = RsaHelper.EncryptByX509(data, PublicKeyCer);
            //     var a2 = RsaHelper.Encrypt(data, PublicKeyPem);
            //
            //     var d1 = RsaHelper.Decrypt(a1, PrivateKeyPkcs8);
            //     var d2 = RsaHelper.Decrypt(a2, PrivateKeyPkcs8);
            //
            //     d1.ShouldBe(d2);
            //
            //     var signature = RsaHelper.Sign(data, PrivateKeyPkcs8);
            //     RsaHelper.VerifyByX509(data, signature, PublicKeyCer)
            //         .ShouldBe(true);
            //     RsaHelper.Verify(data, signature, PublicKeyPem)
            //         .ShouldBe(true);
            // }
        }
    }
}