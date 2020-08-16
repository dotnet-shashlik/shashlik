using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shashlik.Utils.Common;
using Shouldly;
using System.Collections.Concurrent;
using System.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.Utils.Test
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
