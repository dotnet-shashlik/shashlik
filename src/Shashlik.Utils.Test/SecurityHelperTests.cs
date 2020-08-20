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
    public class SecurityHelperTests
    {
        [Fact]
        public void MD5Test()
        {
            "123123".Md532().ShouldBe("4297f44b13955235245b2497399d7a93");
        }

        [Fact]
        public void HMACSHA256()
        {
            SecurityHelper.HMACSHA256("a", "123123", Encoding.UTF8)
                .ShouldBe("b2c8a0f3df7463ae9d10d0c11e2be145a07477355bdc24b163b4ccaad1038752");

            SecurityHelper.HMACSHA256Base64("a", "123123", Encoding.UTF8)
                .ShouldBe("ssig8990Y66dENDBHivhRaB0dzVb3CSxY7TMqtEDh1I=");
        }

        [Fact]
        public void sha256()
        {
            SecurityHelper.Sha256("123123").ShouldBe("96cae35ce8a9b0244178bf28e4966c2ce1b8385723a96a6b838858cdd6ca0a1e");
        }
    }
}
