using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用装配器
    /// </summary>
    public interface IServiceProviderAutowire : IAutowire
    {
        void Configure(IKernelServiceProvider kernelServiceProvider);
    }
}