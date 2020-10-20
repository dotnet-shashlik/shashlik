using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Text.Json;

namespace Shashlik.Utils.Test
{
    public class TypeExtensionsTests
    {
        public interface ITest
        {
        }

        public class TestA : ITest
        {
            public TestB B { get; set; }
        }

        public class TestB : TestA
        {
        }

        [Fact]
        public void Clone_test()
        {
            TestA a = new TestA
            {
                B = new TestB()
            };

            for (int i = 0; i < 1; i++)
            {
                var cloneA = a.Clone();
                ReferenceEquals(a, cloneA).ShouldBeFalse();
                ReferenceEquals(a.B, cloneA.B).ShouldBeFalse();
            }
        }

        [Fact]
        public void GetAllBaseTypes_test()
        {
            var types = typeof(TestB).GetAllBaseTypes();

            types.Count.ShouldBe(1);
            types.First().ShouldBe(typeof(TestA));
        }

        [Fact]
        public void MapToDictionary()
        {
            var obj = new
            {
                first = $"用户11111，提交了新的实名认证申请。",
                keyword1 = $"等待管理员审核",
                keyword2 = DateTime.Now.ToStringyyyyMMddHHmm(),
                remark = $"请及时登录后台管理系统处理。",
                address = new
                {
                    provice = "四川省",
                    city = "成都市",
                    area = "金牛区"
                },
                tags = new[]
                {
                    new
                    {
                        title = "成熟",
                        score = 1
                    },
                    new
                    {
                        title = "稳重",
                        score = 2
                    }
                },
                friends = new[] {"张三", "李四"},
                dic = new Dictionary<object, object> {{1, "1"}},
                dic1 = new Hashtable {{"a", 1}}
            };

            var dic = obj.MapToDictionary();
            dic.Count.ShouldBe(9);
            dic.ContainsKey("first").ShouldBeTrue();
            dic["first"].ShouldBe("用户11111，提交了新的实名认证申请。");
        }

        struct TestStruct
        {
        }

        [Fact]
        public void IsSimpleTypeTest()
        {
            typeof(int).IsSimpleType().ShouldBeTrue();
            typeof(System.Reflection.BindingFlags).IsSimpleType().ShouldBeTrue();
            typeof(int?).IsSimpleType().ShouldBeTrue();
            typeof(string).IsSimpleType().ShouldBeTrue();
            typeof(System.Reflection.BindingFlags?).IsSimpleType().ShouldBeTrue();
            typeof(Guid).IsSimpleType().ShouldBeTrue();
            typeof(Guid?).IsSimpleType().ShouldBeTrue();
            typeof(DateTime).IsSimpleType().ShouldBeTrue();
            typeof(DateTime?).IsSimpleType().ShouldBeTrue();
            typeof(DateTimeOffset).IsSimpleType().ShouldBeTrue();
            typeof(DateTimeOffset?).IsSimpleType().ShouldBeTrue();
            typeof(TimeSpan).IsSimpleType().ShouldBeTrue();
            typeof(TimeSpan?).IsSimpleType().ShouldBeTrue();

            // 自定义的结构体视为复杂类型
            typeof(TestStruct).IsSimpleType().ShouldBeFalse();

            (new { }).GetType().IsSimpleType().ShouldBeFalse();
            typeof(TypeExtensionsTests).IsSimpleType().ShouldBeFalse();
        }


        [Fact]
        public void JsonElementGetValueTests()
        {
            
            var model = new TestJsonToClass
            {
                Int1 = 1,
                Int2 = null,
                D1 = 0.1,
                Dt1 = DateTime.Now,
                Guid1 = Guid.NewGuid(),
                Dto1 = DateTimeOffset.Now,
                Dto2 = DateTimeOffset.Now,
                Enum1 = TestJsonToClass.TestJsonToEnum.Male,
            };

            var json = model.ToJson();
            
            {
                var model2 = JsonSerializer.Deserialize<TestJsonToClass>(json);
                model2.Int1.ShouldBe(model.Int1);
                model2.Int2.ShouldBe(model.Int2);
                model2.D1.ShouldBe(model.D1);
                model2.D2.ShouldBe(model.D2);
                model2.Dt1.ShouldBe(model.Dt1);
                model2.Dt2.ShouldBe(model.Dt2);
                model2.Guid1.ShouldBe(model.Guid1);
                model2.Guid2.ShouldBe(model.Guid2);
                model2.Dto1.ShouldBe(model.Dto1);
                model2.Dto2.ShouldBe(model.Dto2);
                model2.Enum1.ShouldBe(model.Enum1);
                model2.Enum2.ShouldBe(model.Enum2);
            }
            
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                jsonElement.GetProperty("Int1").GetValue<int>().ShouldBe(model.Int1);
                jsonElement.GetProperty("Int2").GetValue<int?>().ShouldBe(model.Int2);
                jsonElement.GetProperty("D1").GetValue<double>().ShouldBe(model.D1);
                jsonElement.GetProperty("D2").GetValue<double?>().ShouldBe(model.D2);
                jsonElement.GetProperty("Dt1").GetValue<DateTime>().ShouldBe(model.Dt1);
                jsonElement.GetProperty("Dt2").GetValue<DateTime?>().ShouldBe(model.Dt2);
                jsonElement.GetProperty("Guid1").GetValue<Guid>().ShouldBe(model.Guid1);
                jsonElement.GetProperty("Guid2").GetValue<Guid?>().ShouldBe(model.Guid2);
                jsonElement.GetProperty("Dto1").GetValue<DateTimeOffset>().ShouldBe(model.Dto1);
                jsonElement.GetProperty("Dto2").GetValue<DateTimeOffset?>().ShouldBe(model.Dto2);
                var v=jsonElement.GetProperty("Enum1").GetValue<TestJsonToClass.TestJsonToEnum>();
                jsonElement.GetProperty("Enum1").GetValue<TestJsonToClass.TestJsonToEnum>().ShouldBe(model.Enum1);
                jsonElement.GetProperty("Enum2").GetValue<TestJsonToClass.TestJsonToEnum?>().ShouldBe(model.Enum2);
            }
        }

        public class TestJsonToClass
        {
            public int Int1 { get; set; }
            public int? Int2 { get; set; }

            public double D1 { get; set; }
            public double? D2 { get; set; }

            public DateTime Dt1 { get; set; }
            public DateTime? Dt2 { get; set; }

            public Guid Guid1 { get; set; }
            public Guid? Guid2 { get; set; }

            public DateTimeOffset Dto1 { get; set; }
            public DateTimeOffset? Dto2 { get; set; }

            public TestJsonToEnum Enum1 { get; set; }
            public TestJsonToEnum? Enum2 { get; set; }

            public enum TestJsonToEnum
            {
                Male = 1,
                Female = 2
            }
        }
    }
}