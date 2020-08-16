using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel.Dependency
{
    public interface IConditionFilterAddProvider
    {
        /// <summary>
        /// 根据条件进行过滤并注册服务
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <param name="services"></param>
        /// <param name="rootConfiguration"></param>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        void FilterAdd(
            IEnumerable<ShashlikServiceDescriptor> serviceDescriptors,
            IServiceCollection services,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment);
    }
}
