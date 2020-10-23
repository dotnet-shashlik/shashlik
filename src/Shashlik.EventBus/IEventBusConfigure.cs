using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Kernel;

namespace Shashlik.EventBus
{
    /// <summary>
    /// event bus 自动装配,主要是配置cap
    /// </summary>
    public interface IEventBusConfigure : IAutowire
    {
        void Configure(CapOptions capOptions);
    }
}