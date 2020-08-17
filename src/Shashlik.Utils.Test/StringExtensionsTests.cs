using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Common;
using System.Threading;
using Newtonsoft.Json.Linq;
using Shashlik.Utils.RazorFormat;
using Newtonsoft.Json;

namespace Shashlik.Utils.Test
{
    public class StringExtensionsTests
    {

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
                    new KeyValuePair<string, object>("openid","oleaE55VwSmo_qO6tjA-0gOj1vUI"),
                    new KeyValuePair<string, object>("type", "wxlogpush"),
                    new KeyValuePair<string, object>("attach", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("empty", null),
                });

                res1.ShouldBe($"http://www.baidu.com?code=cae29605fe284afabb4edd1d9cbc1527&openid=oleaE55VwSmo_qO6tjA-0gOj1vUI&type=wxlogpush&attach=cae29605fe284afabb4edd1d9cbc1527");
            }

            {
                string url = "http://www.baidu.com";
                var res1 = url.UrlArgsCombine(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("code","cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("openid","oleaE55VwSmo_qO6tjA-0gOj1vUI"),
                    new KeyValuePair<string, object>("type", "wxlogpush"),
                    new KeyValuePair<string, object>("attach", "cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("empty", null),
                });

                res1.ShouldBe($"http://www.baidu.com?code=cae29605fe284afabb4edd1d9cbc1527&openid=oleaE55VwSmo_qO6tjA-0gOj1vUI&type=wxlogpush&attach=cae29605fe284afabb4edd1d9cbc1527");
            }

            {
                string url = null;
                var res1 = url.UrlArgsCombine(new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>("code","cae29605fe284afabb4edd1d9cbc1527"),
                    new KeyValuePair<string, object>("openid","oleaE55VwSmo_qO6tjA-0gOj1vUI"),
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

            var model = new UserTestModel
            {
                Age = 18,
                Birthday = new DateTime(2000, 1, 1),
                Money = 1.11111,
                Company = new UserTestModel._Company
                {
                    CompanyName = "test company",
                    Address = new UserTestModel._Company._Address { Code = 1 }
                }
            };


            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}"
                .RazorFormat(model)
                .ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}{model.Company.CompanyName}{model.Company.Address.Code.ToString("d6")}");

            string value = null;
            value.RazorFormat(model).ShouldBeNull();
            "".RazorFormat(model).ShouldBeNullOrWhiteSpace();
            UserTestModel nullModel = null;
            "@{Age}@{Birthday}@{Money}".RazorFormat(nullModel).ShouldBe("@{Age}@{Birthday}@{Money}");

            "@{age}".RazorFormat(model).ShouldBe("@{age}");
            "@empty".RazorFormat(model).ShouldBe("@empty");

            try
            {
                "@{Age|d2|f2}".RazorFormat(model);
            }
            catch (Exception ex)
            {
                ex.ShouldNotBeNull();
            }

            try
            {
                "@{Company|ggggggg}".RazorFormat(model);
            }
            catch (Exception ex)
            {
                ex.ShouldNotBeNull();
            }

            "@{Age}@{Birthday}@{Money|f2}".RazorFormat(model).ShouldBe($"{model.Age}{model.Birthday}{model.Money.ToString("f2")}");
            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}".RazorFormat(model).ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}");

            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}"
                .RazorFormat(model)
                .ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}{model.Company.CompanyName}{model.Company.Address.Code.ToString("d6")}");
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
