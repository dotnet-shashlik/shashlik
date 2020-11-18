using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 服务注册条件过滤支持接口
    /// </summary>
    public interface IFilterProvider
    {
        /// <summary>
        /// 根据条件进行服务过滤
        /// </summary>
        /// <param name="services"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        void DoFilter(
            IServiceCollection services,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment);
    }
}