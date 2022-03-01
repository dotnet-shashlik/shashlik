using System;
using System.Security.Cryptography;
using System.Text;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test.HelperTests
{
    public class EncryptTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EncryptTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void DesEncryptTest()
        {
            var key = "12345678";
            var iv = "12345678";

            {
                var data = DesHelper.Encrypt("DES加密", key, iv);
                data.ShouldBe("lkXACZz387lOk9xiKpCOeg==");
                DesHelper.Decrypt(data, key, iv).ShouldBe("DES加密");
            }

            {
                var data = DesHelper.Encrypt("DES加密", key, null, PaddingMode.PKCS7, CipherMode.ECB);
                DesHelper.Decrypt(data, key, null, PaddingMode.PKCS7, CipherMode.ECB).ShouldBe("DES加密");
            }
        }

        [Fact]
        public void DesErrorTest()
        {
            Should.Throw<Exception>(() => { DesHelper.Encrypt("DES加密", "123", "12345678"); });
            Should.Throw<Exception>(() => { DesHelper.Encrypt("DES加密", "12345678", "123"); });
            Should.Throw<Exception>(() => { DesHelper.Decrypt("lkXACZz387lOk9xiKpCOeg==", "123", "12345678"); });
            Should.Throw<Exception>(() => { DesHelper.Decrypt("lkXACZz387lOk9xiKpCOeg==", "12345678", "123"); });
        }

        [Fact]
        public void AesEncrypt()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var key = "1234567890123456";
            var iv = "1234567890123456";
            {
                var data = AesHelper.Encrypt("AES加密", key, iv);
                data.ShouldBe("B2zgIp4Wvi/SohcgcqQn+Q==");
                AesHelper.Decrypt(data, key, iv).ShouldBe("AES加密");
            }

            {
                var data = AesHelper.Encrypt("AES加密", key, null, PaddingMode.PKCS7, CipherMode.ECB);
                AesHelper.Decrypt(data, key, null, PaddingMode.PKCS7, CipherMode.ECB).ShouldBe("AES加密");
            }
        }


        [Fact]
        public void AesErrorTest()
        {
            Should.Throw<Exception>(() => { AesHelper.Encrypt("AES加密", "123", "1234567890123456"); });
            Should.Throw<Exception>(() => { AesHelper.Encrypt("AES加密", "1234567890123456", "123"); });
            Should.Throw<Exception>(() => { AesHelper.Decrypt("B2zgIp4Wvi/SohcgcqQn+Q==", "123", "1234567890123456"); });
            Should.Throw<Exception>(() => { AesHelper.Decrypt("B2zgIp4Wvi/SohcgcqQn+Q==", "1234567890123456", "123"); });
        }

        [Fact]
        public void TripleDesTest()
        {
            var str = "123123";
            var password = "698ac4d6b9be40a180d65849";
            var iv = "12345678";
            password.Length.ShouldBe(24);
            _testOutputHelper.WriteLine(password);

            var encoded = TripleDesHelper.Encrypt(str, password, iv: iv);
            _testOutputHelper.WriteLine(encoded);
            var decoded = TripleDesHelper.Decrypt(encoded, password, iv: iv);
            decoded.ShouldBe(str);
        }
    }
}