using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Xunit.Abstractions;

namespace Shashlik.Utils.Test
{
    public class WaitingTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public WaitingTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        void wait_test()
        {
            var fourMinAgo = DateTime.Now.AddMinutes(-4).ToString("O");
            _testOutputHelper.WriteLine(fourMinAgo);
        }
    }
}
