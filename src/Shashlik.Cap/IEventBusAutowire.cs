using DotNetCore.CAP;
using Shashlik.Kernel;

namespace Shashlik.Cap
{
    /// <summary>
    /// event bus 自动装配,主要是配置cap
    /// </summary>
    public interface IEventBusAutowire : IAutowire
    {
        void Configure(CapOptions capOptions);
    }
}