using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shashlik.Utils.Common;
using Shouldly;
using System.Collections.Concurrent;
using System.Linq;
using Shashlik.Utils.Common.SnowFlake;

namespace Shashlik.Utils.Test
{
    public class RandomTests
    {
        [Fact]
        void getId_test()
        {
            var i = new IdWorker(1, 1).NextId();
            var s = i.ToString("D19");
        }
    }
}
