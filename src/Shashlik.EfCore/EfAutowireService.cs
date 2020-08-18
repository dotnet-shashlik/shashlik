using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowire;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 自动注册嵌套事务, 自动注册ef实体类
    /// </summary>
    public class EfAutowireService : IAutowireConfigureServices
    {
        public EfAutowireService(ILogger<EfAutowireService> logger)
        {
            Logger = logger;
        }

        ILogger<EfAutowireService> Logger { get; }

        public void ConfigureServices(IKernelService kernelService, IConfiguration rootConfiguration)
        {
            kernelService.AddEfEntityMappings(rootConfiguration);
            var dbContextTypes = AssemblyHelper.GetFinalSubTypes<ShashlikDbContext>(kernelService.ScanFromDependencyContext);
            if (dbContextTypes.IsNullOrEmpty())
                return;

            foreach (var item in dbContextTypes)
            {
                if (!kernelService.Services.Any(r => r.ServiceType == item.GetType()))
                    Logger.LogWarning($"{item} can't configure, please make sure invoke AddDbContext<>(...) before AddShashlik(...)");

                kernelService.Services.TryAddScoped(typeof(IEfNestedTransaction<>).MakeGenericType(item),
                    typeof(DefaultEfNestedTransaction<>).MakeGenericType(item));
            }
        }
    }
}
