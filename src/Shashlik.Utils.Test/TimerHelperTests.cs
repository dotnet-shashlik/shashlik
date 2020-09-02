using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using System.Threading;
using Shashlik.Utils.Helpers;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class TimerHelperTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TimerHelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void SetTimeout_test(int i)
        {
            int origin = i;
            TimerHelper.SetTimeout(() => { i++; }, TimeSpan.FromSeconds(1));

            Thread.Sleep(2 * 1000);

            i.ShouldBe(origin + 1);
        }

        [Fact]
        public void SetInterval_test()
        {
            var source = new CancellationTokenSource();
            var counter = 0;
            TimerHelper.SetInterval(() =>
            {
                counter++;
                _testOutputHelper.WriteLine(counter.ToString());
            }, TimeSpan.FromSeconds(1), source.Token);

            while (true)
            {
                if (counter != 10) continue;
                source.Cancel();
                break;
            }

            counter.ShouldBe(10);
        }
    }
}