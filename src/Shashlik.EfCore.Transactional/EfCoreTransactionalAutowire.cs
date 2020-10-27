using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.EfCore.Transactional
{
    public class EfCoreTransactionalAutowire : IServiceAutowire
    {
        public void Configure(IKernelServices kernelService)
        {
            // 增加全局拦截器
            kernelService.Services.ConfigureDynamicProxy(r =>
            {
                r.Interceptors.AddTyped<TransactionalAttribute>(
                    method => method.IsDefinedAttribute<TransactionalAttribute>(true)
                );
            });
        }
    }
}