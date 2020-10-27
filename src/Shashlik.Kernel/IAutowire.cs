using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel
{
    /// <summary>
    /// 自动装配基类
    /// </summary>
    public interface IAutowire : ITransient
    {
    }

    /// <summary>
    /// 服务装配器
    /// </summary>
    public interface IServiceAutowire : IAutowire
    {
        void Configure(IKernelServices kernelServices);
    }

    /// <summary>
    /// 应用装配器
    /// </summary>
    public interface IServiceProviderAutowire : IAutowire
    {
        void Configure(IKernelServiceProvider kernelConfigure);
    }
}