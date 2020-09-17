using System;
using System.Collections.Generic;
using System.Linq;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;

namespace Shashlik.Utils.Test.HelperTests
{
    public class RandomHelperTests
    {
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
            for (var i = 0; i < 1000; i++)
            {
                var code = RandomHelper.GetRandomCode(10);
                code.ShouldNotBeEmpty();
                code.Length.ShouldBe(10);
            }
        }

        [Fact]
        public void GetRandomNumberTest()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var codes = RandomHelper.GetRandomNum(50, 1, 2);
            });
            for (var i = 0; i < 1000; i++)
            {
                var codes = RandomHelper.GetRandomNum(50, 1, 300);
                codes.ShouldNotBeEmpty();
                codes.Count.ShouldBe(50);
                codes.Distinct().Count().ShouldBe(50);
            }
        }
    }
}