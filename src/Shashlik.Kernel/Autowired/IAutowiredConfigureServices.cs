namespace Shashlik.Kernel.Autowired
{
    /// <summary>
    /// 自动装配服务
    /// </summary>
    public interface IAutowiredConfigureServices
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelService"></param>
        void ConfigureServices(IKernelServices kernelService);
    }
}
