using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    public interface IEventBusConfigureService
    {
        void Configure(CapOptions capOptions);
    }
}
