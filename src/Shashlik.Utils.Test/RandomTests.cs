using System.Collections.Generic;
using Xunit;
using Shashlik.Utils.Common;
using Shouldly;

namespace Shashlik.Utils.Test
{
    public class SnowflakeIdTests
    {
        [Fact]
        void getId_test()
        {
            var worker = new SnowflakeIdHelper(1, 1);

            HashSet<long> ids = new HashSet<long>();

            for (int i = 0; i < 1000000; i++)
            {
                ids.Add(worker.NextId());
            }

            ids.Count.ShouldBe(1000000);
        }
    }
}
