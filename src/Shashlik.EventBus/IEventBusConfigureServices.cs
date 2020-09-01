using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EventBus
{
    /// <summary>
    /// event bus 自动装配,主要是配置cap
    /// </summary>
    public interface IEventBusConfigureServices
    {
        void Configure(CapOptions capOptions);
    }
}