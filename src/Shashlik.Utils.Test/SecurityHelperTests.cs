using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Shouldly;
using Guc.Utils.Extensions;
using Guc.Utils.Common;
using System.Threading;

namespace Guc.Utils.Test
{
    public class SecurityHelperTests
    {
        [Fact]
        public void MD5Test()
        {
            "123123".Md532().ToUpper().ShouldBe("4297F44B13955235245B2497399D7A93");
        }

        [Fact]
        public void AesTest()
        {
            var a = SecurityHelper.HMACSHA256("a", "123123", Encoding.ASCII);
        }
    }
}
