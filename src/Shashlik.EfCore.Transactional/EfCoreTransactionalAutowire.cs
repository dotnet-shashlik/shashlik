using System;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Helpers;

namespace Shashlik.EfCore.Transactional
{
    /**
     *    使用AspectCore作为代理拦截器, 需要额外配置ServiceProviderFactory:
     *    Host.CreateDefaultBuilder()
     *        .UseServiceProviderFactory(new DynamicProxyServiceProviderFactory())
     * 
     */
    public class EfCoreTransactionalAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            // 增加全局拦截器
            kernelService.Services.ConfigureDynamicProxy(r =>
            {
                r.Interceptors.AddTyped<TransactionalAttribute>(
                    p => p.GetCustomAttributes(typeof(TransactionalAttribute), true).Any()
                );
            });
        }
    }
}