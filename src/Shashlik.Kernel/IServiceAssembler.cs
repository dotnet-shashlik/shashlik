namespace Shashlik.Kernel
{
    /// <summary>
    /// 服务装配器
    /// </summary>
    public interface IServiceAssembler : IAssembler
    {
        void Configure(IKernelServices kernelServices);
    }
}