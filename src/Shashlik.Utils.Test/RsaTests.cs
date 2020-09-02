using Shouldly;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Helpers;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class RsaTests
    {
        static RsaTests()
        {
            PublicKeyCer = File.ReadAllText(@"./rsatest.demokey/x509_public.cer");
            PublicKeyPem = File.ReadAllText(@"./rsatest.demokey/rsa_publick_key.pem");
            PrivateKeyPkcs8 = File.ReadAllText(@"./rsatest.demokey/pkcs8_rsa_private_key.pem");
            PrivateKeyPkcs1 = File.ReadAllText(@"./rsatest.demokey/rsa_private_key.pem");
        }

        private static string PublicKeyCer { get; }
        private static string PublicKeyPem { get; }
        private static string PrivateKeyPkcs8 { get; }
        private static string PrivateKeyPkcs1 { get; }

        [Fact]
        public void IntegrationTest()
        {
            var data = "jb3rhg03pj5yl4756860h-590hu4-9hu90-hno[3h945u-5468upmobvg37546-[8pu5m7p[68ih90";

            {
                var a1 = RsaHelper.EncryptByX509(data, PublicKeyCer, Encoding.UTF8, RSAEncryptionPadding.Pkcs1);
                var a2 = RsaHelper.Encrypt(data, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                    RSAEncryptionPadding.Pkcs1, true);

                var d1 = RsaHelper.Decrypt(a1, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                    RSAEncryptionPadding.Pkcs1, true);
                var d2 = RsaHelper.Decrypt(a2, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                    RSAEncryptionPadding.Pkcs1, true);
                var d3 = RsaHelper.Decrypt(a1, PrivateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8,
                    RSAEncryptionPadding.Pkcs1, true);

                d1.ShouldBe(d2);
                d1.ShouldBe(d3);

                var signature = RsaHelper.Sign(data, PrivateKeyPkcs8, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                    HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
                RsaHelper.VerifyByX509(data, signature, PublicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5,
                        RSASignaturePadding.Pss)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signature, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                        HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
                    .ShouldBe(true);

                var signature1 = RsaHelper.Sign(data, PrivateKeyPkcs1, Rsa.RSAKeyType.Pkcs1, Encoding.UTF8,
                    HashAlgorithmName.MD5, RSASignaturePadding.Pss, true);
                RsaHelper.VerifyByX509(data, signature1, PublicKeyCer, Encoding.UTF8, HashAlgorithmName.MD5,
                        RSASignaturePadding.Pss)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signature1, PublicKeyPem, Rsa.RSAKeyType.Pkcs8, Encoding.UTF8,
                        HashAlgorithmName.MD5, RSASignaturePadding.Pss, true)
                    .ShouldBe(true);
            }

            {
                var a1 = RsaHelper.EncryptByX509(data, PublicKeyCer);
                var a2 = RsaHelper.Encrypt(data, PublicKeyPem);

                var d1 = RsaHelper.Decrypt(a1, PrivateKeyPkcs8);
                var d2 = RsaHelper.Decrypt(a2, PrivateKeyPkcs8);

                d1.ShouldBe(d2);

                var signature = RsaHelper.Sign(data, PrivateKeyPkcs8);
                RsaHelper.VerifyByX509(data, signature, PublicKeyCer)
                    .ShouldBe(true);
                RsaHelper.Verify(data, signature, PublicKeyPem)
                    .ShouldBe(true);
            }
        }
    }
}