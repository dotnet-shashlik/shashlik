namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动配置项目
    /// </summary>
    public interface IAutowireConfigure : Shashlik.Kernel.Dependency.ISingleton
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelConfigure"></param>
        /// <param name="configuration"></param>
        void Configure(IKernelConfigure kernelConfigure);
    }
}
