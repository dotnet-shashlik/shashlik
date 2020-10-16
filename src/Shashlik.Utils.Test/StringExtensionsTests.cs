using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.RazorFormat;
using Shashlik.Utils.Helpers.Encrypt;
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
            var api = "http://api.test.yusuinet.com/openapi/test";
            // appId
            var appId = "12ae01c0d6fc4d8aa34dab379a3f5dcb";
            // 密钥
            var secret = "ed37a865b0484a01a91f653b";
            // 计算当前时间戳
            var timestamp = "1600504614";
            // 使用guid生成32位随机字符
            var nonce = "742b0467aa364ff491d163d400a17209";
            // 使用guid生成32位请求id
            var requestId = "e28ccc72720c4b599ef84ac7100d96fd";
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
            var hash = HashHelper.HmacSha256Base64(json, secret);
            _testOutputHelper.WriteLine(hash);
        }


        [Fact]
        public void ConfidentialData_test()
        {
            string phone = "13628452323";
            phone.ConfidentialData(3, 3).ShouldBe("136****323");
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
        }

        [Fact]
        public void RazorFormatTest()
        {
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(null));
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(new ErrorFormatter()));
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(new DuplicateFormatter()));

            var model = new UserTestModel
            {
                Age = 18,
                Birthday = new DateTime(2000, 1, 1),
                Money = 1.11111,
                Company = new UserTestModel._Company
                {
                    CompanyName = "test company",
                    Address = new UserTestModel._Company._Address {Code = 1}
                }
            };


            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}@{NotMatch}"
                .RazorFormat(model)
                .ShouldBe(
                    $"{model.Age:d5}{model.Birthday:yyyy-MM-dd HH:mm:ss}{model.Money:f2}{model.Company.CompanyName}{model.Company.Address.Code:d6}@{{NotMatch}}");

            ((string) null).RazorFormat(model).ShouldBeNull();
            "".RazorFormat(model).ShouldBeNullOrWhiteSpace();
            "@{Age}@{Birthday}@{Money}".RazorFormat((UserTestModel) null).ShouldBe("@{Age}@{Birthday}@{Money}");

            "@{age}".RazorFormat(model).ShouldBe("@{age}");
            "@empty".RazorFormat(model).ShouldBe("@empty");

            "@{Age|d2|f2}".RazorFormat(model);

            "@{Detail}".RazorFormat(new UserTestModel()).ShouldBe("");

            "@{Company|ggggggg}".RazorFormat(model);

            "@{Age}@{Birthday}@{Money|f2}".RazorFormat(model)
                .ShouldBe($"{model.Age}{model.Birthday}{model.Money:f2}");
            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}".RazorFormat(model).ShouldBe(
                $"{model.Age:d5}{model.Birthday:yyyy-MM-dd HH:mm:ss}{model.Money:f2}");

            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}"
                .RazorFormat(model)
                .ShouldBe(
                    $"{model.Age:d5}{model.Birthday:yyyy-MM-dd HH:mm:ss}{model.Money:f2}{model.Company.CompanyName}{model.Company.Address.Code:d6}");
            var switchFormatString = "@{Gender|switch(0:未知|1:男性|2:女性|null:不男不女|empty:空|default:未知)}";
            switchFormatString.RazorFormat(new {Gender = 2}).ShouldBe("女性");
            //switchFormatString.RazorFormat(new {}).ShouldBe("不男不女");
            switchFormatString.RazorFormat(new {Gender = ""}).ShouldBe("空");
            switchFormatString.RazorFormat(new {Gender = 7}).ShouldBe("未知");
            "@{Gender|switch(0:未知)}".RazorFormat(new {Gender = 7}).ShouldBe("7");
            try
            {
                "@{Gender|switch(0未知)}".RazorFormat(new {Gender = 7});
            }
            catch (Exception e)
            {
                e.ShouldNotBeNull();
            }
        }

        [Fact]
        public void UriTest()
        {
            var uri1 = new Uri("https://www.baidu.com/a");
            var uri2 = new Uri("https://www.baidu.com:5000/b");
            var uri3 = new Uri("http://www.baidu.com/a");
            var uri4 = new Uri("http://www.baidu.com:5000/b");
        }

        [Fact]
        public void Encoding()
        {
            var en = System.Text.Encoding.GetEncoding("UTF-8");
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

    internal class ErrorFormatter : IFormatter
    {
        public string Action { get; }

        public string Format(string value, string expression)
        {
            throw new NotImplementedException();
        }
    }

    internal class DuplicateFormatter : IFormatter
    {
        public string Action { get; } = "switch";

        public string Format(string value, string expression)
        {
            throw new NotImplementedException();
        }
    }
}