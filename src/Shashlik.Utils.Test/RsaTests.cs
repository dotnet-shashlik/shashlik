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
            var data = Guid.NewGuid().ToString();

            {
                var a1 = RsaHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var a2 = RsaHelper.FromPublicKey(PublicKeyPem, RSAKeyType.Pkcs8, true)
                    .EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var d1 = RsaHelper.FromPrivateKey(PrivateKeyPkcs8, RSAKeyType.Pkcs8, true)
                    .DecryptBigData(a1, RSAEncryptionPadding.OaepSHA256);
                var d2 = RsaHelper.FromPrivateKey(PrivateKeyPkcs8, RSAKeyType.Pkcs8, true)
                    .DecryptBigData(a2, RSAEncryptionPadding.OaepSHA256);
                var d3 = RsaHelper.FromPrivateKey(PrivateKeyPkcs1, RSAKeyType.Pkcs1, true)
                    .DecryptBigData(a1, RSAEncryptionPadding.OaepSHA256);

                d1.ShouldBe(d2);
                d1.ShouldBe(d3);

                var signature = RsaHelper.FromPrivateKey(PrivateKeyPkcs8, RSAKeyType.Pkcs8, true)
                    .SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                var signature1 = RsaHelper.FromPrivateKey(PrivateKeyPkcs1, RSAKeyType.Pkcs1, true)
                    .SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                RsaHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().VerifySignData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RsaHelper.FromPublicKey(PublicKeyPem, RSAKeyType.Pkcs8, true)
                    .VerifySignData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RsaHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().VerifySignData(data, signature1, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RsaHelper.FromPublicKey(PublicKeyPem, RSAKeyType.Pkcs8, true)
                    .VerifySignData(data, signature1, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);
            }
        }
    }
}