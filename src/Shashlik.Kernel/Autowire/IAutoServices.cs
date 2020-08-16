using Microsoft.Extensions.Configuration;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配服务
    /// </summary>
    public interface IAutoServices
    {
        /// <summary>
        /// 服务配置
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="configuration"></param>
        void ConfigureServices(IKernelService kernelBuilder, IConfiguration configuration);
    }
}
