namespace Shashlik.Kernel
{
    /// <summary>
    /// 应用装配器
    /// </summary>
    public interface IServiceProviderAssembler : IAssembler
    {
        void Configure(IKernelServiceProvider kernelServiceProvider);
    }
}