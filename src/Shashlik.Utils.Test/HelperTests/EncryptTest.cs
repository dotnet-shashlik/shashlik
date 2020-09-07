using System.Text;
using Shashlik.Utils.Helpers.Encrypt;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    public class EncryptTest
    {
        [Fact]
        public void DesEncryptTest()
        {

            var key = "12345678";
            var iv = "12345678";
            var data = DesHelper.Encrypt("DES加密", key, iv);
            data.ShouldBe("lkXACZz387lOk9xiKpCOeg==");
        }

        [Fact]
        public void DesDecrypt()
        {
            var key = "12345678";
            var iv = "12345678";
            var data = DesHelper.Decrypt("lkXACZz387lOk9xiKpCOeg==", key, iv);
            data.ShouldBe("DES加密");
        }

        [Fact]
        public void AesEncrypt()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var key = "1234567890123456";
            var iv = "1234567890123456";
            var data = AesHelper.Encrypt("AES加密", key, iv);
            data.ShouldBe("B2zgIp4Wvi/SohcgcqQn+Q==");
        }

        [Fact]
        public void AesDecrypt()
        {
            var key = "1234567890123456";
            var iv = "1234567890123456";
            var data = AesHelper.Decrypt("B2zgIp4Wvi/SohcgcqQn+Q==", key, iv);
            data.ShouldBe("AES加密");
        }

        [Fact]
        public void BCryptTest()
        {
            var password = "password";
            var hash = BCryptHelper.Hash(password);
            var result = BCryptHelper.Verify(password, hash);
            result.ShouldBeTrue();
        }
    }
}