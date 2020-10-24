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
using System.Dynamic;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class TypeExtensionsTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        #region Base

        private struct TestStruct
        {
        }
        
        internal class TestAttribute : Attribute
        {
            
        }

        private interface ITest
        {
        }

        internal class TestA : ITest
        {
            public TestB B { get; set; }
        }

        internal class TestB : TestA
        {
            public TestB()
            {
                
            }

            public TestB(string str, int number)
            {
                
            }
        }
        
        internal class TestJsonToClass
        {
            public int Int1 { get; set; }
            public int? Int2 { get; set; }

            [Test]
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
            
            public List<string> StrList { get; set; }

            public enum TestJsonToEnum
            {
                Male = 1,
                Female = 2
            }
        }
        
        private object _testObject = new
        {
            first = "用户11111，提交了新的实名认证申请。",
            keyword1 = "等待管理员审核",
            keyword2 = DateTime.Now.ToStringyyyyMMddHHmm(),
            remark = "请及时登录后台管理系统处理。",
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

        public TypeExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        #endregion
        
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
            var dic = _testObject.MapToDictionary();
            dic.Count.ShouldBe(9);
            dic.ContainsKey("first").ShouldBeTrue();
            dic["first"].ShouldBe("用户11111，提交了新的实名认证申请。");
        }

        

        [Fact]
        public void IsSimpleTypeTest()
        {
            typeof(int).IsSimpleType().ShouldBeTrue();
            typeof(BindingFlags).IsSimpleType().ShouldBeTrue();
            typeof(int?).IsSimpleType().ShouldBeTrue();
            typeof(string).IsSimpleType().ShouldBeTrue();
            typeof(BindingFlags?).IsSimpleType().ShouldBeTrue();
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
                StrList = new List<string>{"str1", "str2"}
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
            //_testOutputHelper.WriteLine(json);
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

                bool exists;
                object value;
                (exists, value) = jsonElement.GetPropertyValue("Int1");
                exists.ShouldBeTrue();
                value.ShouldBe(model.Int1);
                (exists, value) = jsonElement.GetPropertyValue("______");
                exists.ShouldBeFalse();
                value.ShouldBeNull();
                (exists, value) = jsonElement.GetPropertyValue("StrList.1");
                exists.ShouldBeTrue();
                value.ShouldBe(model.StrList.Last());
                
                
                jsonElement.GetProperty("Int1").GetValue<int>().ShouldBe(model.Int1);
                jsonElement.GetProperty("Int1").GetValue(typeof(int)).ShouldBe(model.Int1);
                jsonElement.GetProperty("Int2").GetValue<int?>().ShouldBe(model.Int2);
                jsonElement.GetProperty("Int2").GetValue(typeof(int?)).ShouldBe(model.Int2);
                jsonElement.GetProperty("D1").GetValue<double>().ShouldBe(model.D1);
                jsonElement.GetProperty("D1").GetValue(typeof(double)).ShouldBe(model.D1);
                jsonElement.GetProperty("D2").GetValue<double?>().ShouldBe(model.D2);
                jsonElement.GetProperty("D2").GetValue(typeof(double?)).ShouldBe(model.D2);
                jsonElement.GetProperty("Dt1").GetValue<DateTime>().ShouldBe(model.Dt1);
                jsonElement.GetProperty("Dt1").GetValue(typeof(DateTime)).ShouldBe(model.Dt1);
                jsonElement.GetProperty("Dt2").GetValue<DateTime?>().ShouldBe(model.Dt2);
                jsonElement.GetProperty("Dt2").GetValue(typeof(DateTime)).ShouldBe(model.Dt2);
                jsonElement.GetProperty("Guid1").GetValue<Guid>().ShouldBe(model.Guid1);
                jsonElement.GetProperty("Guid1").GetValue(typeof(Guid)).ShouldBe(model.Guid1);
                jsonElement.GetProperty("Guid2").GetValue<Guid?>().ShouldBe(model.Guid2);
                jsonElement.GetProperty("Guid2").GetValue(typeof(Guid?)).ShouldBe(model.Guid2);
                jsonElement.GetProperty("Dto1").GetValue<DateTimeOffset>().ShouldBe(model.Dto1);
                jsonElement.GetProperty("Dto1").GetValue(typeof(DateTimeOffset)).ShouldBe(model.Dto1);
                jsonElement.GetProperty("Dto2").GetValue<DateTimeOffset?>().ShouldBe(model.Dto2);
                jsonElement.GetProperty("Dto2").GetValue(typeof(DateTimeOffset?)).ShouldBe(model.Dto2);
                jsonElement.GetProperty("Enum1").GetValue<TestJsonToClass.TestJsonToEnum>();
                jsonElement.GetProperty("Enum1").GetValue(typeof(TestJsonToClass.TestJsonToEnum));
                jsonElement.GetProperty("Enum1").GetValue<TestJsonToClass.TestJsonToEnum>().ShouldBe(model.Enum1);
                jsonElement.GetProperty("Enum1").GetValue(typeof(TestJsonToClass.TestJsonToEnum)).ShouldBe(model.Enum1);
                jsonElement.GetProperty("Enum2").GetValue<TestJsonToClass.TestJsonToEnum?>().ShouldBe(model.Enum2);
                jsonElement.GetProperty("Enum2").GetValue(typeof(TestJsonToClass.TestJsonToEnum?)).ShouldBe(model.Enum2);

                var dic = jsonElement.MapToDictionary();
                dic.ShouldNotBeEmpty();
                dic.ContainsKey("Guid1").ShouldBeTrue();
                dic.ContainsKey("Enum2").ShouldBeTrue();
            }
        }

        [Fact]
        public void ConstructorTests()
        {
            typeof(TestB).GetDeclaredConstructor(null).ShouldNotBeNull();
            typeof(TestB).GetDeclaredConstructor(new []{typeof(string), typeof(int)}).ShouldNotBeNull();
        }

        [Fact]
        public void GetPropertyTests()
        {
            bool exists;
            object value;
            (exists, value) = _testObject.GetPropertyValue("keyword1");
            exists.ShouldBeTrue();
            value.ShouldBe("等待管理员审核");
            (exists, value) = _testObject.GetPropertyValue("address.city");
            exists.ShouldBeTrue();
            value.ShouldBe("成都市");
            (exists, value) = _testObject.GetPropertyValue("friends.1");
            exists.ShouldBeTrue();
            value.ShouldBe("李四");
            
            var dic = new Dictionary<string, string>()
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };
            (exists, value) = dic.GetPropertyValue("key1");
            exists.ShouldBeTrue();
            value.ShouldBe(dic["key1"]);
            
            JToken jt = new JObject();
            jt["key"] = "value";
            (exists, value) = jt.GetPropertyValue("key");
            exists.ShouldBeTrue();
            value.ShouldBe(jt["key"]);
            
            var ja = new JArray();
            ja.Add(jt);
            (exists, value) = ja.GetPropertyValue("0.key");
            exists.ShouldBeTrue();
            value.ToString().ShouldBe(jt["key"].ToString());
        }

        [Fact]
        public void GetDefaultValueTests()
        {
            typeof(TestB).GetDefaultValue().ShouldBeNull();
            typeof(ITest).GetDefaultValue().ShouldBeNull();
            typeof(int).GetDefaultValue().ShouldBe(0);
            typeof(string).GetDefaultValue().ShouldBeNull();
        }

        [Fact]
        public void ParseToTests()
        {
            "1".ParseTo<int>().ShouldBe(1);
            "0010".ParseTo<long>().ShouldBe(10);
            1.ParseTo<TestJsonToClass.TestJsonToEnum>().ShouldBe(TestJsonToClass.TestJsonToEnum.Male);
            "false".ParseTo<bool>().ShouldBeFalse();
            
            "1".TryParse<int>(out var intValue).ShouldBeTrue();
            intValue.ShouldBe(1);
            "0010".TryParse<long>(out var longValue).ShouldBeTrue();
            longValue.ShouldBe(10);
            1.TryParse<TestJsonToClass.TestJsonToEnum>(out var enumValue).ShouldBeTrue();
            enumValue.ShouldBe(TestJsonToClass.TestJsonToEnum.Male);
            "false".TryParse<bool>(out var boolValue).ShouldBeTrue();
            boolValue.ShouldBeFalse();
            
            "".TryParse<bool>(out var errorValue).ShouldBeFalse();
            
            "1".TryParse(typeof(int), out var intValue2).ShouldBeTrue();
            intValue2.ShouldBe(1);
            "0010".TryParse(typeof(long), out var longValue2).ShouldBeTrue();
            longValue2.ShouldBe(10);
            1.TryParse(typeof(TestJsonToClass.TestJsonToEnum), out var enumValue2).ShouldBeTrue();
            enumValue2.ShouldBe(TestJsonToClass.TestJsonToEnum.Male);
            "false".TryParse(typeof(bool), out var boolValue2).ShouldBeTrue();
            boolValue2.ShouldBe(false);
            
            "".TryParse(typeof(bool), out var errorValue2).ShouldBeFalse();
        }

        [Fact]
        public void TypeCheckTests()
        {
            typeof(List<>).IsCollectionType().ShouldBeTrue();
            typeof(Dictionary<,>).IsCollectionType().ShouldBeTrue();
            typeof(HashSet<>).IsCollectionType().ShouldBeTrue();
            
            typeof(string).IsCollectionType().ShouldBeTrue();
            
            typeof(int).IsCollectionType().ShouldBeFalse();
            typeof(TestA).IsCollectionType().ShouldBeFalse();
            
            typeof(List<>).IsDelegate().ShouldBeFalse();
            typeof(Dictionary<,>).IsDelegate().ShouldBeFalse();
            typeof(HashSet<>).IsDelegate().ShouldBeFalse();
            typeof(int).IsDelegate().ShouldBeFalse();
            typeof(string).IsDelegate().ShouldBeFalse();
            typeof(TestA).IsDelegate().ShouldBeFalse();
            
            typeof(Action).IsDelegate().ShouldBeTrue();
            typeof(Func<>).IsDelegate().ShouldBeTrue();

            var member = typeof(TestJsonToClass).GetMember(nameof(TestJsonToClass.D1)).First();
            member.ShouldNotBeNull();
            member.IsDefinedAttribute<TestAttribute>(true).ShouldBeTrue();
            member.IsDefinedAttribute(typeof(TestAttribute), true).ShouldBeTrue();
            
            typeof(TestB).GetInterfaces(true).ShouldNotBeEmpty();
            typeof(TestB).GetInterfaces(false).ShouldBeEmpty();
        }
    }
}