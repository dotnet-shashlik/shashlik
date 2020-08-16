namespace Shashlik.Kernel.Automatic
{
    /// <summary>
    /// 自动装配服务
    /// </summary>
    public interface IAutoConfigure : Shashlik.Kernel.Dependency.ISingleton
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        void Configure(IKernelConfigure kernelConfigure);
    }
}
