using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test.HelperTests
{
    public class RandomHelperTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RandomHelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void getId_test()
        {
            var worker = new SnowflakeId(1, 1);

            HashSet<long> ids = new HashSet<long>();

            for (int i = 0; i < 1000000; i++)
            {
                ids.Add(worker.NextId());
            }

            ids.Count.ShouldBe(1000000);
        }

        [Fact]
        public void GetRandomCodeTest()
        {
            for (int length = 1; length <= 32; length++)
            {
                for (var i = 0; i < 10; i++)
                {
                    var code = RandomHelper.GetRandomCode(length);
                    _testOutputHelper.WriteLine(code);
                    code.ShouldNotBeEmpty();
                    code.Length.ShouldBe(length);
                }
            }
        }

        [Fact]
        public void NextTest()
        {
            for (var i = 0; i < 100; i++)
            {
                var code = RandomHelper.Next(10, 100);
                code.ShouldBeGreaterThanOrEqualTo(10);
                code.ShouldBeLessThan(100);
                _testOutputHelper.WriteLine(code.ToString());
            }
        }
    }
}