using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Common;
using System.Threading;
using System.Linq;
using System.Reflection;
using System.Collections;

namespace Shashlik.Utils.Test
{
    public class TypeExtensionsTests
    {
        public interface ITest { }

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

            for (int i = 0; i < 1000; i++)
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
                tags = new[] {
                    new {
                        title="成熟",
                        score=1
                    },
                    new {
                        title="稳重",
                        score=2
                    }
                },
                friends = new[] { "张三", "李四" },
                dic = new Dictionary<object, object> { { 1, "1" } },
                dic1 = new Hashtable { { "a", 1 } }
            };

            var dic = obj.MapToDictionary();
            dic.Count.ShouldBe(4);
            dic.ContainsKey("first").ShouldBeTrue();
            dic["first"].ShouldBe("用户11111，提交了新的实名认证申请。");
        }

        struct TestStruct { }

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
    }
}
