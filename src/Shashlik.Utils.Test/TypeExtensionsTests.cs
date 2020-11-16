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
using System.ComponentModel;
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
            public string Str { get; set; }
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

        public class TestJsonToClass
        {
            public int Int1 { get; set; }
            public int? Int2 { get; set; }

            public long Long { get; set; }
            public float Float { get; set; }
            public decimal Decimal { get; set; }
            public short Short { get; set; }
            public uint Uint { get; set; }
            public ushort Ushort { get; set; }
            public ulong Ulong { get; set; }
            public byte Byte { get; set; }
            public sbyte Sbyte { get; set; }
            public char Char { get; set; }

            [Test] public double D1 { get; set; }
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

            public TestJsonToClass InnerClass { get; set; }

            public enum TestJsonToEnum
            {
                [Description("男")] Male = 1,
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
            friends = new[] { "张三", "李四" },
            dic = new Dictionary<object, object> { { 1, "1" } },
            dic1 = new Hashtable { { "a", 1 } },
            jObject = new
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
                friends = new[] { "张三", "李四" },
                dic = new Dictionary<object, object> { { 1, "1" } },
                dic1 = new Hashtable { { "a", 1 } },
            }.ToJson().DeserializeJson<JObject>(),
            jElement = new
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
                friends = new[] { "张三", "李四" },
                // text json not support Serialize: Dictionary<object, object>
                // dic = new Dictionary<object, object> {{1, "1"}},
                dic1 = new Hashtable { { "a", 1 } },
            }.ToJsonWithTextJson().DeserializeJsonWithTextJson<JsonElement>()
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

            var jObject = new JObject { ["test"] = "test", ["array"] = new JArray("value1", "value2") };

            var jDic = jObject.MapToDictionary();
            jDic.ContainsKey("test").ShouldBeTrue();
            jDic["test"].ShouldBe("test");
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
                StrList = new List<string> { "str1", "str2" },
                InnerClass = new TestJsonToClass
                {
                    Int1 = 1,
                    Int2 = null,
                    D1 = 0.1,
                    Dt1 = DateTime.Now,
                    Guid1 = Guid.NewGuid(),
                    Dto1 = DateTimeOffset.Now,
                    Dto2 = DateTimeOffset.Now,
                    Enum1 = TestJsonToClass.TestJsonToEnum.Male,
                    StrList = new List<string> { "str1", "str2" },
                }
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
                (exists, value) = jsonElement.GetPropertyValue("Long");
                exists.ShouldBeTrue();
                value.ShouldBe(model.Long);
                (exists, value) = jsonElement.GetPropertyValue("Decimal");
                exists.ShouldBeTrue();
                value.ShouldBe(model.Decimal);
                (exists, value) = jsonElement.GetPropertyValue("______");
                exists.ShouldBeFalse();
                value.ShouldBeNull();
                (exists, value) = jsonElement.GetPropertyValue("StrList.1");
                exists.ShouldBeTrue();
                value.ShouldBe(model.StrList.Last());

                {
                    Should.Throw<Exception>(() => jsonElement.GetProperty("D1").GetValue<bool>());

                    jsonElement.GetValue<long>("Long").ShouldBe(model.Long);
                    jsonElement.GetValue<float>("Float").ShouldBe(model.Float);
                    jsonElement.GetValue<decimal>("Decimal").ShouldBe(model.Decimal);
                    jsonElement.GetValue<short>("Short").ShouldBe(model.Short);
                    jsonElement.GetValue<uint>("Uint").ShouldBe(model.Uint);
                    jsonElement.GetValue<ulong>("Ulong").ShouldBe(model.Ulong);
                    jsonElement.GetValue<ushort>("Ushort").ShouldBe(model.Ushort);
                    jsonElement.GetValue<byte>("Byte").ShouldBe(model.Byte);
                    jsonElement.GetValue<sbyte>("Sbyte").ShouldBe(model.Sbyte);
                    jsonElement.GetValue<char>("Char").ShouldBe(model.Char);
                    jsonElement.GetValue<List<string>>("StrList").ShouldBe(model.StrList);

                    jsonElement.GetProperty("Int1").GetValue<int>().ShouldBe(model.Int1);
                    jsonElement.GetProperty("Int1").GetValue<long>().ShouldBe(model.Int1);
                    jsonElement.GetProperty("Int1").GetValue<decimal>().ShouldBe(model.Int1);
                    jsonElement.GetValue<int>("Int1").ShouldBe(model.Int1);
                    jsonElement.GetProperty("Int1").GetValue(typeof(int)).ShouldBe(model.Int1);
                    jsonElement.GetValue(typeof(int), "Int1").ShouldBe(model.Int1);

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
                    jsonElement.GetProperty("Enum1").GetValue(typeof(TestJsonToClass.TestJsonToEnum))
                        .ShouldBe(model.Enum1);
                    jsonElement.GetProperty("Enum2").GetValue<TestJsonToClass.TestJsonToEnum?>().ShouldBe(model.Enum2);
                    jsonElement.GetProperty("Enum2").GetValue(typeof(TestJsonToClass.TestJsonToEnum?))
                        .ShouldBe(model.Enum2);


                    jsonElement.GetProperty("InnerClass").GetValue<long>("Long").ShouldBe(model.InnerClass.Long);
                    jsonElement.GetProperty("InnerClass").GetValue<float>("Float").ShouldBe(model.InnerClass.Float);
                    jsonElement.GetProperty("InnerClass").GetValue<decimal>("Decimal").ShouldBe(model.InnerClass.Decimal);
                    jsonElement.GetProperty("InnerClass").GetValue<short>("Short").ShouldBe(model.InnerClass.Short);
                    jsonElement.GetProperty("InnerClass").GetValue<uint>("Uint").ShouldBe(model.InnerClass.Uint);
                    jsonElement.GetProperty("InnerClass").GetValue<ulong>("Ulong").ShouldBe(model.InnerClass.Ulong);
                    jsonElement.GetProperty("InnerClass").GetValue<ushort>("Ushort").ShouldBe(model.InnerClass.Ushort);
                    jsonElement.GetProperty("InnerClass").GetValue<byte>("Byte").ShouldBe(model.InnerClass.Byte);
                    jsonElement.GetProperty("InnerClass").GetValue<sbyte>("Sbyte").ShouldBe(model.InnerClass.Sbyte);
                    jsonElement.GetProperty("InnerClass").GetValue<char>("Char").ShouldBe(model.InnerClass.Char);
                    jsonElement.GetProperty("InnerClass").GetValue<List<string>>("StrList").ShouldBe(model.InnerClass.StrList);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int1").GetValue<int>().ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int1").GetValue<long>().ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int1").GetValue<decimal>().ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetValue<int>("Int1").ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int1").GetValue(typeof(int))
                        .ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetValue(typeof(int), "Int1").ShouldBe(model.InnerClass.Int1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int2").GetValue<int?>().ShouldBe(model.InnerClass.Int2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Int2").GetValue(typeof(int?))
                        .ShouldBe(model.InnerClass.Int2);
                    jsonElement.GetProperty("InnerClass").GetProperty("D1").GetValue<double>().ShouldBe(model.InnerClass.D1);
                    jsonElement.GetProperty("InnerClass").GetProperty("D1").GetValue(typeof(double)).ShouldBe(model.InnerClass.D1);
                    jsonElement.GetProperty("InnerClass").GetProperty("D2").GetValue<double?>().ShouldBe(model.InnerClass.D2);
                    jsonElement.GetProperty("InnerClass").GetProperty("D2").GetValue(typeof(double?))
                        .ShouldBe(model.InnerClass.D2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dt1").GetValue<DateTime>().ShouldBe(model.InnerClass.Dt1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dt1").GetValue(typeof(DateTime))
                        .ShouldBe(model.InnerClass.Dt1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dt2").GetValue<DateTime?>().ShouldBe(model.InnerClass.Dt2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dt2").GetValue(typeof(DateTime))
                        .ShouldBe(model.InnerClass.Dt2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Guid1").GetValue<Guid>().ShouldBe(model.InnerClass.Guid1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Guid1").GetValue(typeof(Guid))
                        .ShouldBe(model.InnerClass.Guid1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Guid2").GetValue<Guid?>().ShouldBe(model.InnerClass.Guid2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Guid2").GetValue(typeof(Guid?))
                        .ShouldBe(model.InnerClass.Guid2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dto1").GetValue<DateTimeOffset>()
                        .ShouldBe(model.InnerClass.Dto1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dto1").GetValue(typeof(DateTimeOffset))
                        .ShouldBe(model.InnerClass.Dto1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dto2").GetValue<DateTimeOffset?>()
                        .ShouldBe(model.InnerClass.Dto2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Dto2").GetValue(typeof(DateTimeOffset?))
                        .ShouldBe(model.InnerClass.Dto2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum1")
                        .GetValue<TestJsonToClass.TestJsonToEnum>();
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum1")
                        .GetValue(typeof(TestJsonToClass.TestJsonToEnum));
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum1")
                        .GetValue<TestJsonToClass.TestJsonToEnum>().ShouldBe(model.InnerClass.Enum1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum1")
                        .GetValue(typeof(TestJsonToClass.TestJsonToEnum))
                        .ShouldBe(model.InnerClass.Enum1);
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum2")
                        .GetValue<TestJsonToClass.TestJsonToEnum?>().ShouldBe(model.InnerClass.Enum2);
                    jsonElement.GetProperty("InnerClass").GetProperty("Enum2")
                        .GetValue(typeof(TestJsonToClass.TestJsonToEnum?))
                        .ShouldBe(model.InnerClass.Enum2);
                }
            }
        }

        [Fact]
        public void ConstructorTests()
        {
            Should.Throw<Exception>(() => typeof(TestB).GetDeclaredConstructor(null).ShouldNotBeNull());
            typeof(TestB).GetDeclaredConstructor().ShouldNotBeNull();
            typeof(TestB).GetDeclaredConstructor(typeof(string), typeof(int)).ShouldNotBeNull();
        }

        [Fact]
        public void GetPropertyTests()
        {
            {
                bool exists;
                object value;

                {
                    (exists, value) = _testObject.GetPropertyValue("absolute_not_exists");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("keyword1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("等待管理员审核");
                    (exists, value) = _testObject.GetPropertyValue("address.city");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成都市");
                    (exists, value) = _testObject.GetPropertyValue("tags.0.title");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成熟");
                    (exists, value) = _testObject.GetPropertyValue("tags.2.title");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("friends.1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("李四");
                    (exists, value) = _testObject.GetPropertyValue("dic.1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("1");
                    (exists, value) = _testObject.GetPropertyValue("dic1.a");
                    exists.ShouldBeTrue();
                    value.ShouldBe(1);
                }

                {
                    (exists, value) = _testObject.GetPropertyValue("jObject.absolute_not_exists");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("jObject.keyword1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("等待管理员审核");
                    (exists, value) = _testObject.GetPropertyValue("jObject.address.city");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成都市");
                    (exists, value) = _testObject.GetPropertyValue("jObject.tags.0.title");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成熟");
                    (exists, value) = _testObject.GetPropertyValue("jObject.tags.2.title");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("jObject.friends.1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("李四");
                    (exists, value) = _testObject.GetPropertyValue("jObject.dic.1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("1");
                    (exists, value) = _testObject.GetPropertyValue("jObject.dic1.a");
                    exists.ShouldBeTrue();
                    value.ShouldBe(1);
                }

                {
                    (exists, value) = _testObject.GetPropertyValue("jElement.absolute_not_exists");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("jElement.keyword1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("等待管理员审核");
                    (exists, value) = _testObject.GetPropertyValue("jElement.address.city");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成都市");
                    (exists, value) = _testObject.GetPropertyValue("jElement.tags.0.title");
                    exists.ShouldBeTrue();
                    value.ShouldBe("成熟");
                    (exists, value) = _testObject.GetPropertyValue("jElement.tags.2.title");
                    exists.ShouldBeFalse();
                    (exists, value) = _testObject.GetPropertyValue("jElement.friends.1");
                    exists.ShouldBeTrue();
                    value.ShouldBe("李四");
                    // (exists, value) = _testObject.GetPropertyValue("jElement.dic.1");
                    // exists.ShouldBeTrue();
                    // value.ShouldBe("1");
                    (exists, value) = _testObject.GetPropertyValue("jElement.dic1.a");
                    exists.ShouldBeTrue();
                    value.ShouldBe(1);
                }


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

            {
                object v = null;
                v.ParseTo<int?>().ShouldBeNull();
            }

            {
                object v = 1;
                v.ParseTo<int?>().ShouldBe(1);
            }

            {
                int v = 1;
                v.ParseTo<int>().ShouldBe(v);
            }
            {
                object v = 1;
                v.ParseTo<int>().ShouldBe(1);
            }
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

        [Fact]
        public void CopyToTests()
        {
            var a = new TestA();
            var b = new TestB()
            {
                Str = "B1"
            };
            var b2 = new TestB()
            {
                Str = "B2"
            };

            a.CopyTo(b);
            a.Str.ShouldBe(b.Str);
            b2.CopyTo(b);
            b.Str.ShouldBe(b2.Str);

            Should.Throw<Exception>(() => a.CopyTo(null));
            a = null;
            Should.Throw<Exception>(() => a.CopyTo(b));
        }

        [Fact]
        public void EnumTests()
        {
            TestJsonToClass.TestJsonToEnum.Male.GetEnumDescription().ShouldBe("男");
            TestJsonToClass.TestJsonToEnum.Female.GetEnumDescription().ShouldBeNullOrWhiteSpace();
        }
    }

    public static class JsonExtensions
    {
        public static string ToJsonWithTextJson(this object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public static T DeserializeJsonWithTextJson<T>(this string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}