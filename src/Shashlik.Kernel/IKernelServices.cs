using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Shashlik.Kernel
{
    /// <summary>
    /// Shashlik 内核 服务配置
    /// </summary>
    public interface IKernelServices
    {
        /// <summary>
        /// 服务集
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// 程序集扫描上下文
        /// </summary>
        DependencyContext ScanFromDependencyContext { get; }

        /// <summary>
        /// 根配置
        /// </summary>
        IConfiguration RootConfiguration { get; }
    }
}