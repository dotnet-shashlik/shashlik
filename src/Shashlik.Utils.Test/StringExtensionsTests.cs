using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
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
            try
            {
                RazorFormatExtensions.Registy(null);
            }
            catch (Exception e)
            {
                e.ShouldNotBe(null);
            }
            try
            {
                RazorFormatExtensions.Registy(new ErrorFormatter());
            }
            catch (Exception e)
            {
                e.ShouldNotBe(null);
            }
            try
            {
                RazorFormatExtensions.Registy(new DuplicateFormatter());
            }
            catch (Exception e)
            {
                e.ShouldNotBe(null);
            }

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


            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}@{NotMatch}"
                .RazorFormat(model)
                .ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}{model.Company.CompanyName}{model.Company.Address.Code.ToString("d6")}@{{NotMatch}}");

            string value = null;
            value.RazorFormat(model).ShouldBeNull();
            "".RazorFormat(model).ShouldBeNullOrWhiteSpace();
            UserTestModel nullModel = null;
            "@{Age}@{Birthday}@{Money}".RazorFormat(nullModel).ShouldBe("@{Age}@{Birthday}@{Money}");

            "@{age}".RazorFormat(model).ShouldBe("@{age}");
            "@empty".RazorFormat(model).ShouldBe("@empty");

            "@{Age|d2|f2}".RazorFormat(model);

            "@{Company|ggggggg}".RazorFormat(model);

            "@{Age}@{Birthday}@{Money|f2}".RazorFormat(model).ShouldBe($"{model.Age}{model.Birthday}{model.Money.ToString("f2")}");
            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}".RazorFormat(model).ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}");

            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}"
                .RazorFormat(model)
                .ShouldBe($"{model.Age.ToString("d5")}{model.Birthday.ToString("yyyy-MM-dd HH:mm:ss")}{model.Money.ToString("f2")}{model.Company.CompanyName}{model.Company.Address.Code.ToString("d6")}");
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
