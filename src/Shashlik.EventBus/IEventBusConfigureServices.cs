using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    public interface IEventBusConfigureServices
    {
        void Configure(CapOptions capOptions);
    }
}
