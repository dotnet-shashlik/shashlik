using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 自动装配基类
    /// </summary>
    [Transient(typeof(IAutowire), RequireRegistryInheritedChain = true)]
    public interface IAutowire
    {
    }
}