using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配服务
    /// </summary>
    public interface IAutowireConfigureServices
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="rootConfiguration"></param>
        void ConfigureServices(IKernelServices kernelService);
    }
}
