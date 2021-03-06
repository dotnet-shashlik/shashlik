﻿
using System;
using System.Diagnostics;
using Shashlik.Utils.Extensions;
using Shouldly;
using SmartFormat;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.RazorFormat.Test
{
    public class RazorFormatTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RazorFormatTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private UserTestModel model = new UserTestModel
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

        [Fact]
        public void Test()
        {
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(null));
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(new ErrorFormatter()));
            Should.Throw<Exception>(() => RazorFormatExtensions.Registry(new DuplicateFormatter()));


            Should.Throw<Exception>(() =>
                "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}@{NotMatch}"
                    .RazorFormat(model, throwExceptionWhenPropertyNotExists: true));

            "@{Age|d5}@{Birthday|yyyy-MM-dd HH:mm:ss}@{Money|f2}@{Company.CompanyName}@{Company.Address.Code|d6}@{NotMatch}"
                .RazorFormat(model)
                .ShouldBe(
                    $"{model.Age:d5}{model.Birthday:yyyy-MM-dd HH:mm:ss}{model.Money:f2}{model.Company.CompanyName}{model.Company.Address.Code:d6}@{{NotMatch}}");

            "仍然让人人@@{Age|d5}啊啊啊啊啊@{NotMatch}啪啪啪@{Age|d5}嘎嘎嘎@{Birthday|yyyy-MM-dd HH:mm:ss}宝贝宝贝宝贝呢@{Money|f2}惆怅长岑长@{Company.CompanyName}对方的等待@{Company.Address.Code|d6}诶诶诶诶诶@{NotMatch}呵呵呵呵"
                .RazorFormat(model)
                .ShouldBe(
                    $"仍然让人人@@{{Age|d5}}啊啊啊啊啊@{{NotMatch}}啪啪啪{model.Age:d5}嘎嘎嘎{model.Birthday:yyyy-MM-dd HH:mm:ss}宝贝宝贝宝贝呢{model.Money:f2}惆怅长岑长{model.Company.CompanyName}对方的等待{model.Company.Address.Code:d6}诶诶诶诶诶@{{NotMatch}}呵呵呵呵");

            string emptyStr = null;
            emptyStr.RazorFormat(model).ShouldBeNull();

            "".RazorFormat(model).ShouldBeNullOrWhiteSpace();
            "@{Age}@{Birthday}@{Money}".RazorFormat((UserTestModel) null).ShouldBe("@{Age}@{Birthday}@{Money}");

            "@{age}".RazorFormat(model).ShouldBe("@{age}");
            "@empty".RazorFormat(model).ShouldBe("@empty");

            "@{Age|d2|f2}".RazorFormat(model);

            "@{Detail}".RazorFormat(new UserTestModel()).ShouldBe("");

            "@{Company|ggggggg}".RazorFormat(model).ShouldBe(typeof(UserTestModel._Company).ToString());
            "@{Company.CompanyName|ggggggg}".RazorFormat(model).ShouldBe("test company");

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
            Should.Throw<Exception>(() => "@{Gender|switch(0未知)}".RazorFormat(new {Gender = 7}));
        }

        [Fact]
        public void FormatPipelineTest()
        {
            // -> 0000000018
            // -> ...0000000018
            // -> ...0000000018...
            // -> ...00###
            "@{Age|stringFormat(D10)-addPrefix(...)-addSuffix(...)-subIfTooLong(5,###)}"
                .RazorFormat(model)
                .ShouldBe("...00###");

            // -> 0000000018
            // -> 00018
            "@{Age|stringFormat(D10)-substr(5)}"
                .RazorFormat(model)
                .ShouldBe("00018");

            // -> 0000000018
            // -> 00
            "@{Age|stringFormat(D10)-substr( 5, 2)}"
                .RazorFormat(model)
                .ShouldBe("00");

            // -> 0000000018
            // -> ABC0000000018
            // -> aBC0000000018
            "@{Age|stringFormat(D10)-addPrefix(ABC)-toggleCase(firstLower)}"
                .RazorFormat(model)
                .ShouldBe("aBC0000000018");
        }

        private readonly int _performanceCount = 10_0000;

        [Fact]
        public void RazorFormatPerformanceTest()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _performanceCount; i++)
            {
                "仍然让人人@@{Age|d5}啊啊啊啊啊@{NotMatch}啪啪啪@{Age|d5}嘎嘎嘎@{Birthday|yyyy-MM-dd HH:mm:ss}宝贝宝贝宝贝呢@{Money|f2}惆怅长岑长@{Company.CompanyName}对方的等待@{Company.Address.Code|d6}诶诶诶诶诶@{NotMatch}呵呵呵呵"
                    .RazorFormat(model);
            }

            sw.Stop();

            _testOutputHelper.WriteLine($"RazorFormat 执行{_performanceCount}次总计耗时: {sw.ElapsedMilliseconds / 1000M}秒");
        }

        [Fact]
        public void SmartFormatPerformanceTest()
        {
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < _performanceCount; i++)
            {
                var res = Smart.Format(
                    "仍然让人人{Age:d5}啊啊啊啊啊啪啪啪{Age:d5}嘎嘎嘎{Birthday:yyyy-MM-dd HH:mm:ss}宝贝宝贝宝贝呢{Money:f2}惆怅长岑长{Company.CompanyName}对方的等待{Company.Address.Code:d6}诶诶诶诶诶呵呵呵呵",
                    model
                );
            }

            sw.Stop();

            _testOutputHelper.WriteLine($"SmartFormat 执行{_performanceCount}次总计耗时: {sw.ElapsedMilliseconds / 1000M}秒");
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

        public object Format(object value, string expression)
        {
            throw new NotImplementedException();
        }
    }

    internal class DuplicateFormatter : IFormatter
    {
        public string Action { get; } = "switch";

        public object Format(object value, string expression)
        {
            throw new NotImplementedException();
        }
    }
}