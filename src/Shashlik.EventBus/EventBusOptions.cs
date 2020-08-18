using Shashlik.Kernel.Autowire.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    [AutoOptions("Shashlik:EventBus")]
    public class EventBusOptions
    {
        public string DefaultGroup { get; set; }

        public string Version { get; set; }

        public int SucceedMessageExpiredAfter { get; set; }

        public int FailedRetryInterval { get; set; }

        public int FailedRetryCount { get; set; }

        public int ConsumerThreadCount { get; set; }
    }
}
