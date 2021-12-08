using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Thread.Sleep(1000);
            var worker1 = new SnowflakeId(1, unixTimeMilliseconds);
            var worker2 = new SnowflakeId(2, unixTimeMilliseconds);
            var worker3 = new SnowflakeId(3, unixTimeMilliseconds);
            var worker4 = new SnowflakeId(4, unixTimeMilliseconds);

            HashSet<long> ids = new HashSet<long>();

            for (int i = 0; i < 1000000; i++)
                ids.Add(worker1.NextId());
            for (int i = 0; i < 1000000; i++)
                ids.Add(worker2.NextId());
            for (int i = 0; i < 1000000; i++)
                ids.Add(worker3.NextId());
            for (int i = 0; i < 1000000; i++)
                ids.Add(worker4.NextId());

            ids.Count.ShouldBe(1000000 * 4);

            Should.Throw<ArgumentException>((() => new SnowflakeId(-54, 1)));
            Should.Throw<ArgumentException>((() => new SnowflakeId(1, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeMilliseconds())));
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

            Should.Throw<Exception>(() => RandomHelper.Next(10, 1));
            RandomHelper.Next(10, 10).ShouldBe(10);
        }
    }
}