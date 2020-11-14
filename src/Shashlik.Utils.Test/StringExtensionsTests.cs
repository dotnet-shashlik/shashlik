using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;
using Xunit.Abstractions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shashlik.Utils.Test
{
    public class StringExtensionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StringExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void test()
        {
            // var api = "http://api.test.yusuinet.com/openapi/test";
            // appId
            var appId = "12ae01c0d6fc4d8aa34dab379a3f5dcb";
            // 密钥
            var secret = "ed37a865b0484a01a91f653b";
            // 计算当前时间戳
            var timestamp = "1600504614";
            // 使用guid生成32位随机字符
            var nonce = "742b0467aa364ff491d163d400a17209";
            // 使用guid生成32位请求id
            // var requestId = "e28ccc72720c4b599ef84ac7100d96fd";
            // 创建接口调用对象
            var obj = new
            {
                appId,
                timestamp,
                nonce,
                data = new
                {
                    year = 2020,
                    momth = 1
                }
            };

            var json = JsonSerializer.Serialize(obj);
            _testOutputHelper.WriteLine(json);
            _testOutputHelper.WriteLine("");

            // 使用3des加密,ECB+PKCS7
            var encrypted = TripleDesHelper.Encrypt(json, secret, CipherMode.ECB, PaddingMode.PKCS7);
            _testOutputHelper.WriteLine(encrypted);
            _testOutputHelper.WriteLine("");

            // HmacSha256 hash
            var hash = HashHelper.HMACSHA256(json, secret);
            _testOutputHelper.WriteLine(hash);
        }


        [Fact]
        public void ConfidentialData_test()
        {
            string phone = "13000000000";
            phone.ConfidentialData(3, 3).ShouldBe("130****000");
            "".ConfidentialData(3, 3).ShouldBe("");
            "abc".ConfidentialData(4, 4).ShouldBe("abc****abc");
            Should.Throw<Exception>(() => "abc".ConfidentialData(-1, 4).ShouldBe("abc"));
            Should.Throw<Exception>(() => "abc".ConfidentialData(4, -1).ShouldBe("abc"));
            phone.ConfidentialData(3, 3, "####").ShouldBe("130####000");
            phone.ConfidentialData(12, 3, "####").ShouldBe($"{phone}####000");
            phone.ConfidentialData(3, 12, "####").ShouldBe($"130####{phone}");
        }

        [Fact]
        public void UrlArgsCombine_test()
        {
            {
                string url = "http://www.baidu.com?code=cae29605fe284afabb4edd1d9cbc1527";
                var res1 = url.UrlArgsCombine(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("openid", "oleaE55VwSmo_qO6tjA-0gOj1vUI"),
                    new KeyValuePair<string, object>("type", "wxlogpush"),
                    new KeyValuePair<string, object>("attach", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("empty", null),
                });

                res1.ShouldBe(
                    $"http://www.baidu.com?code=cae29605fe284afabb4edd1d9cbc1527&openid=oleaE55VwSmo_qO6tjA-0gOj1vUI&type=wxlogpush&attach=cae29605fe284afabb4edd1d9cbc1527");
            }

            {
                string url = "http://www.baidu.com";
                var res1 = url.UrlArgsCombine(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("code", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("openid", "oleaE55VwSmo_qO6tjA-0gOj1vUI"),
                    new KeyValuePair<string, object>("type", "wxlogpush"),
                    new KeyValuePair<string, object>("attach", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("empty", null),
                });

                res1.ShouldBe(
                    $"http://www.baidu.com?code=cae29605fe284afabb4edd1d9cbc1527&openid=oleaE55VwSmo_qO6tjA-0gOj1vUI&type=wxlogpush&attach=cae29605fe284afabb4edd1d9cbc1527");
            }

            {
                string url = null;
                var res1 = url.UrlArgsCombine(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("code", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("openid", "oleaE55VwSmo_qO6tjA-0gOj1vUI"),
                    new KeyValuePair<string, object>("type", "wxlogpush"),
                    new KeyValuePair<string, object>("attach", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("empty", null),
                });
                res1.ShouldBeNullOrWhiteSpace();
            }

            {
                string url = "http://www.baidu.com";
                var res1 = url.UrlArgsCombine(null);
                res1.ShouldBe($"http://www.baidu.com");
            }

            {
                var str = "!@#$%^&*()-=_+123abc中文";
                str.UrlEncode().UrlDecode().ShouldBe(str);
            }
        }

        [Fact]
        public void EqualsTest()
        {
            "abc".EqualsIgnoreCase("ABC").ShouldBeTrue();
            "abcd".EqualsIgnoreCase("Abc").ShouldBeFalse();
            "abc".EqualsIgnoreCase(null).ShouldBeFalse();
        }

        [Fact]
        public void SplitTest()
        {
            "".Split<int>().ShouldBeEmpty();
            "".SplitSkipError<int>().ShouldBeEmpty();
            "1,3,4|5_7,".Split<int>(",", "|", "_").Count.ShouldBe(5);
            "1,3,4|a_7,".SplitSkipError<int>(",", "|", "_").Count.ShouldBe(4);
        }

        [Fact]
        public void ContainsTest()
        {
            string test = null;
            StringExtensions.Contains(test, "Ab", StringComparison.CurrentCultureIgnoreCase).ShouldBeFalse();
            StringExtensions.Contains("test", "Ab", StringComparison.CurrentCultureIgnoreCase).ShouldBeFalse();
            StringExtensions.Contains("ABC", "Ab", StringComparison.CurrentCultureIgnoreCase).ShouldBeTrue();
            StringExtensions.Contains("ABC", "", StringComparison.CurrentCultureIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public void SubLongStringTest()
        {
            var str = "longString123";
            str.SubStringIfTooLong(1).ShouldBe("l...");
            str.SubStringIfTooLong(100).ShouldBe(str);
            string.Empty.SubStringIfTooLong(1).ShouldBe(string.Empty);
        }

        [Fact]
        public void ReplaceTest()
        {
            "AbCd".ReplaceIgnoreCase("ab", "").ShouldBe("Cd");
        }

        [Fact]
        public void MatchTest()
        {
            string str = null;
            str.IsMatch(".*").ShouldBeFalse();
            "abc".IsMatch(".*").ShouldBeTrue();
            "abc".IsMatch("\\d+").ShouldBeFalse();
        }

        [Fact]
        public void EmptyToNullTest()
        {
            "   ".EmptyToNull().ShouldBeNull();
            "\t".EmptyToNull().ShouldBeNull();
            "\n".EmptyToNull().ShouldBeNull();
            "a\t\r\n".EmptyToNull().ShouldNotBeNull();
        }

        [Fact]
        public void FormatTest()
        {
            var str = "str,{0}";
            var formatted = string.Format(str, 1);
            str.Format(1).ShouldBe(formatted);
        }

        [Fact]
        public void StartsAndEndsTests()
        {
            var str = "aBcdEf";
            str.StartsWithIgnoreCase("ab").ShouldBeTrue();
            str.StartsWithIgnoreCase("aC").ShouldBeFalse();
            str.EndsWithIgnoreCase("ef").ShouldBeTrue();
            str.EndsWithIgnoreCase("df").ShouldBeFalse();
        }

        [Fact]
        public void Base64Tests()
        {
            var str = "!@#$%^&*()-=_+123abc中文";
            var base64 = str.Base64Encode(Encoding.UTF8, true);
            base64.Base64Decode(Encoding.UTF8, true).ShouldBe(str);
            base64 = str.Base64Encode(Encoding.UTF8);
            base64.Base64DecodeToBytes().ShouldBe(Encoding.UTF8.GetBytes(str));
        }

        [Fact]
        public void HtmlCodingTest()
        {
            var str = "!@#$%^&*()-=_+123abc中文";
            str.HtmlEncode().HtmlDecode().ShouldBe(str);
        }

        [Fact]
        public void RemoveHtmlTest()
        {
            var html = "<a href=\"https://www.google.com/\">中文（繁體）</a>";
            html.RemoveHtml().ShouldBe("中文（繁體）");
        }

        [Fact]
        public void FirstCaseTest()
        {
            "".UpperFirstCase().ShouldBe("");
            "".LowerFirstCase().ShouldBe("");
            "abc".UpperFirstCase().ShouldBe("Abc");
            "abc".LowerFirstCase().ShouldBe("abc");
            "ABC".UpperFirstCase().ShouldBe("ABC");
            "ABC".LowerFirstCase().ShouldBe("aBC");
        }

        [Fact]
        public void SubStringByTextElementsTest()
        {
            var str = "😉✌✌😉😉😉😉😉😉";
            str.SubStringByTextElements(1, 2).ShouldBe("✌✌");
            str.SubStringByTextElements(0, str.Length + 2).ShouldBe(str);
        }

        public class UserTestModel
        {
            public string Detail { get; set; }
            public int Age { get; set; }
            public DateTime Birthday { get; set; }

            public double Money { get; set; }

            public _Company Company { get; set; }

            public class _Company
            {
                public string CompanyName { get; set; }
                public _Address Address { get; set; }

                public class _Address
                {
                    public int Code { get; set; }
                }
            }
        }
    }
}