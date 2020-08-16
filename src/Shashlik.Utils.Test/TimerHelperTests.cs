using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Common;
using System.Threading;

namespace Shashlik.Utils.Test
{
    public class TimerHelperTests
    {
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
            TimerHelper.SetTimeout(() =>
            {
                i++;

            }, TimeSpan.FromSeconds(1));

            Thread.Sleep(12 * 1000);

            i.ShouldBe(origin + 1);
        }
    }
}
