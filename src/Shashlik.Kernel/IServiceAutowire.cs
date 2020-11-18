using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 服务装配器
    /// </summary>
    public interface IServiceAutowire : IAutowire
    {
        void Configure(IKernelServices kernelServices);
    }
}