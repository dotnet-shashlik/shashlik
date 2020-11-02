using DotNetCore.CAP;
using Shashlik.Kernel;

namespace Shashlik.Cap
{
    /// <summary>
    /// cap 自动装配,主要是配置cap
    /// </summary>
    public interface ICapAutowire : IAutowire
    {
        void Configure(CapOptions capOptions);
    }
}