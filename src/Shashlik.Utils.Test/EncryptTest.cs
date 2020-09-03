using System;
using System.Text;
using Shashlik.Utils.Encrypt;
using Shouldly;
using Xunit;

namespace Guc.Utils.Test
{
    public class EncryptTest
    {
        [Fact]
        public void DesEncryptTest()
        {
            var key = Encoding.UTF8.GetBytes("12345678");
            var iv = Encoding.UTF8.GetBytes("12345678");
            var data = "DES加密".DesEncrypt(key, iv);
            data.ShouldBe("lkXACZz387lOk9xiKpCOeg==");
        }

        [Fact]
        public void DesDecrypt()
        {
            var key = Encoding.UTF8.GetBytes("12345678");
            var iv = Encoding.UTF8.GetBytes("12345678");
            var data = "lkXACZz387lOk9xiKpCOeg==".DesDecrypt(key, iv);
            data.ShouldBe("DES加密");
        }

        [Fact]
        public void AesEncrypt()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var key = Encoding.UTF8.GetBytes("1234567890123456");
            var iv = Encoding.UTF8.GetBytes("1234567890123456");
            var data = "AES加密".AesEncrypt(key, iv);
            data.ShouldBe("B2zgIp4Wvi/SohcgcqQn+Q==");
        }

        [Fact]
        public void AesDecrypt()
        {
            var key = Encoding.UTF8.GetBytes("1234567890123456");
            var iv = Encoding.UTF8.GetBytes("1234567890123456");
            var data = "B2zgIp4Wvi/SohcgcqQn+Q==".AesDecrypt(key, iv);
            data.ShouldBe("AES加密");
        }

        [Fact]
        public void BCryptTest()
        {
            var password = "password";
            var hash = password.BCrypt();
            var result = password.BCryptVerify(hash);
            result.ShouldBeTrue();
        }
    }
}