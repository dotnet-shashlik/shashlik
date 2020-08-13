using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Guc.Utils.Common;
using Shouldly;
using System.Collections.Concurrent;
using System.Linq;
using Guc.Utils.Extensions;

namespace Guc.Utils.Test
{
    public class JsonTests
    {
        [Fact]
        void ser_test()
        {
            var d = new
            {
                Name = "老王",
                Birthday = new DateTime(2000, 1, 1)
            };

            var s = JsonHelper.Serialize(d);
        }
    }
}
