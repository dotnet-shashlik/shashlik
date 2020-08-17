namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配服务
    /// </summary>
    public interface IAutowireConfigure : Shashlik.Kernel.Dependency.ISingleton
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        void Configure(IKernelConfigure kernelConfigure);
    }
}
