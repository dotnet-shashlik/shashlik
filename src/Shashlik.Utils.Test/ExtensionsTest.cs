using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Guc.Utils.Extensions;

namespace Guc.Utils.Test
{
    public class ExtensionsTest
    {
        [Fact]
        public void IsMaxPrecision_test()
        {
            689M.IsMaxPrecision(2).ShouldBeTrue();
            123M.IsMaxPrecision(2).ShouldBeTrue();
            123.11M.IsMaxPrecision(2).ShouldBeTrue();
            123.1M.IsMaxPrecision(2).ShouldBeTrue();
            0.1M.IsMaxPrecision(2).ShouldBeTrue();
            1.111M.IsMaxPrecision(2).ShouldBeFalse();
        }
    }
}
