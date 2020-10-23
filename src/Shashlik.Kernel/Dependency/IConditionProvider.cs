using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Shashlik.Kernel.Dependency
{
    public interface IConditionProvider
    {
        /// <summary>
        /// 根据条件进行过滤并注册服务
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="services"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        void FilterAndRegistryService(
            IEnumerable<ShashlikServiceDescriptor> serviceDescriptors,
            IServiceCollection services,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment);
    }
}