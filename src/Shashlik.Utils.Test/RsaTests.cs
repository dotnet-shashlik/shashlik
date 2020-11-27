using System;
using Shouldly;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Shashlik.Utils.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class RsaTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        static RsaTests()
        {
            PublicKeyCer = File.ReadAllText(@"./rsatest.demokey/x509_public.cer");
            PublicKeyPem = File.ReadAllText(@"./rsatest.demokey/rsa_public_key.pem");
            PrivateKeyPkcs8 = File.ReadAllText(@"./rsatest.demokey/pkcs8_rsa_private_key.pem");
            PrivateKeyPkcs1 = File.ReadAllText(@"./rsatest.demokey/rsa_private_key.pem");
            var pfxBytes = File.ReadAllBytes(@"./rsatest.demokey/test.pfx");
            PfxBase64 = Convert.ToBase64String(pfxBytes);
        }

        public RsaTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
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
            using var cer = RSAHelper.LoadX509FromFileBase64(PfxBase64, "123123");
            var encoded = cer.GetRSAPublicKey().EncryptBigData(data, RSAEncryptionPadding.Pkcs1);
            var decoded = cer.GetRSAPrivateKey().DecryptBigData(encoded, RSAEncryptionPadding.Pkcs1);
            decoded.ShouldBe(data);
        }


        [Fact]
        public void IntegrationTest()
        {
            var data = Guid.NewGuid().ToString();

            {
                var a1 = RSAHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var a2 = RSAHelper.FromPem(PublicKeyPem)
                    .EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var a3 = RSAHelper.FromPem(PublicKeyPem)
                    .Encrypt(data, RSAEncryptionPadding.Pkcs1);
                var a4 = RSAHelper.FromPem(PublicKeyPem)
                    .EncryptBigDataWithSplit(data, RSAEncryptionPadding.Pkcs1);
                var d1 = RSAHelper.FromPem(PrivateKeyPkcs8)
                    .DecryptBigData(a1, RSAEncryptionPadding.OaepSHA256);
                var d28 = RSAHelper.FromPem(PrivateKeyPkcs8)
                    .DecryptBigData(a2, RSAEncryptionPadding.OaepSHA256);
                var d21 = RSAHelper.FromPem(PrivateKeyPkcs1)
                    .DecryptBigData(a1, RSAEncryptionPadding.OaepSHA256);
                var d3 = RSAHelper.FromPem(PrivateKeyPkcs1)
                    .Decrypt(a3, RSAEncryptionPadding.Pkcs1);
                var d4 = RSAHelper.FromPem(PrivateKeyPkcs1)
                    .DecryptBigDataWithSplit(a4, RSAEncryptionPadding.Pkcs1);

                d1.ShouldBe(d28);
                d1.ShouldBe(d21);
                d1.ShouldBe(d3);

                var signature = RSAHelper.FromPem(PrivateKeyPkcs8)
                    .SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                var signature1 = RSAHelper.FromPem(PrivateKeyPkcs1)
                    .SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                RSAHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().VerifySignData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RSAHelper.FromPem(PublicKeyPem)
                    .VerifySignData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RSAHelper.LoadX509FromPublicCertificate(PublicKeyCer)
                    .GetRSAPublicKey().VerifySignData(data, signature1, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);

                RSAHelper.FromPem(PublicKeyPem)
                    .VerifySignData(data, signature1, HashAlgorithmName.SHA256, RSASignaturePadding.Pss)
                    .ShouldBe(true);
            }

            {
                using var rsa = RSAHelper.FromPem(PublicKeyPem);
                Should.Throw<Exception>(() => rsa.ToPem(true, true));
            }

            {
                // xml 私钥导出测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var encrypted = rsa.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var xml = rsa.ToXml(true);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(xml);
                using var rsa1 = RSAHelper.FromXml(xml);
                rsa1.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }

            {
                // xml 公钥导出测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var xml = rsa.ToXml(false);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(xml);
                using var rsa1 = RSAHelper.FromXml(xml);
                var encrypted = rsa1.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                rsa.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }

            {
                // pem pkcs1 导出导入测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var encrypted = rsa.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var pem = rsa.ToPem(true, false);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(pem);
                using var rsa1 = RSAHelper.FromPem(pem);
                rsa1.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }

            {
                // pem pkcs1 导出导入测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var pem = rsa.ToPem(false, false);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(pem);
                using var rsa1 = RSAHelper.FromPem(pem);
                var encrypted = rsa1.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                rsa.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }

            {
                // pem pkcs8 导出导入测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var encrypted = rsa.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                var pem = rsa.ToPem(true, true);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(pem);
                using var rsa1 = RSAHelper.FromPem(pem);
                rsa1.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }

            {
                // pem pkcs8 导出导入测试
                using var rsa = RSAHelper.FromPem(PrivateKeyPkcs8);
                var pem = rsa.ToPem(false, true);
                _testOutputHelper.WriteLine("###############");
                _testOutputHelper.WriteLine(pem);
                using var rsa1 = RSAHelper.FromPem(pem);
                var encrypted = rsa1.EncryptBigData(data, RSAEncryptionPadding.OaepSHA256);
                rsa.DecryptBigData(encrypted, RSAEncryptionPadding.OaepSHA256).ShouldBe(data);
            }
        }

        [Fact]
        public void ErrorTest()
        {
            RSA rsa = null;
            Should.Throw<Exception>(() =>
                rsa.VerifySignData("a", "b", HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            Should.Throw<Exception>(() =>
                rsa.SignDataGetBytes("a", HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
        }
    }
}