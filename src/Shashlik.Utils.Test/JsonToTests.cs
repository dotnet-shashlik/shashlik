using System;
using Shashlik.Utils.Extensions;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test
{
    public class JsonToTests
    {
        private class TestClass
        {
            public string TestStr { get; set; }
        }
        
        [Fact]
        public void Tests()
        {
            var test = new TestClass()
            {
                TestStr = "test"
            };
            test.ToJson().ShouldNotBeNullOrEmpty();
            
            var camelCaseJson = test.ToJsonWithCamelCase();
            camelCaseJson.ShouldNotBeNullOrEmpty();
            camelCaseJson.ShouldContain("testStr");

            var obj = camelCaseJson.DeserializeJson<TestClass>();
            obj.ShouldNotBeNull();
            obj.TestStr.ShouldBe(test.TestStr);
        }

        [Fact]
        public void ErrorTests()
        {
            TestClass test = null;
            Should.Throw<Exception>(() => test.ToJson());
            Should.Throw<Exception>(() => test.ToJsonWithCamelCase());
        }
    }
}