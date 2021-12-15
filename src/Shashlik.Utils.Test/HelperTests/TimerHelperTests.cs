using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Shashlik.Utils.Helpers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test.HelperTests
{
    public class TimerHelperTests
    {
        private readonly ITestOutputHelper _testOutputHelper;


        public TimerHelperTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public async Task SetTimeout_test(int i)
        {
            ConcurrentBag<object> list = new ConcurrentBag<object>();

            for (int j = 1; j <= i; j++)
            {
                TimerHelper.SetTimeout(() => { list.Add(new object()); }, TimeSpan.FromSeconds(i));
            }

            await Task.Delay(i * 1000 + 1000);

            list.Count.ShouldBe(i);
        }

        [Fact]
        public async Task SetInterval_test()
        {
            var source = new CancellationTokenSource();
            ConcurrentBag<object> list = new ConcurrentBag<object>();
            TimerHelper.SetInterval(() => { list.Add(new object()); }, TimeSpan.FromSeconds(1), source.Token);

            while (true)
            {
                if (list.Count != 10) continue;
                source.Cancel();
                break;
            }

            await Task.Delay(1000);

            list.Count.ShouldBe(10);
        }

        [Fact]
        public void TimerErrorTest()
        {
            var counter = 0;
            Should.Throw<Exception>(() => { TimerHelper.SetInterval(() => { counter++; }, TimeSpan.FromSeconds(-1)); });

            Should.Throw<Exception>(() => { TimerHelper.SetTimeout(() => { counter++; }, TimeSpan.FromSeconds(-1)); });
            Should.Throw<Exception>(() =>
            {
                TimerHelper.SetTimeout(() => { counter++; },
                    new DateTimeOffset(2009, 1, 1, 1, 1, 1, TimeSpan.Zero));
            });

            counter.ShouldBe(0);
        }
    }
}