using System;
using System.Linq;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowired;
using Shashlik.Utils.Helpers;

namespace Shashlik.EfCore.Transactional
{
    public class EfCoreTransactionalConfigure : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            // 增加全局拦截器
            kernelService.Services.ConfigureDynamicProxy(r =>
            {
                r.Interceptors.AddTyped<TransactionalAttribute>(
                    p => p.GetCustomAttributes(typeof(TransactionalAttribute), true).Any()
                );
            });

            var dic = AssemblyHelper.GetTypesByAttributes<DefaultTransactionalAttribute>();
            if (dic.Count > 1)
                throw new InvalidOperationException($"Find too many {typeof(DefaultTransactionalAttribute)}. ");
            if (dic.Count == 1 && dic.First().Key.IsAbstract)
                throw new InvalidOperationException(
                    $"{typeof(DefaultTransactionalAttribute)} can not be used on abstract class. ");
            if (dic.Count == 1)
                TransactionalAttribute.DefaultDbContextType = dic.First().Key;
        }
    }
}