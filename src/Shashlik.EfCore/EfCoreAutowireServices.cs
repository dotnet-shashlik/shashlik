using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Shashlik.Kernel;
using Shashlik.Kernel.Autowire;
using Shashlik.Kernel.Autowire.Attributes;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Shashlik.Kernel.Autowired;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 自动注册嵌套事务, 自动注册ef实体类
    /// </summary>
    public class EfCoreAutowireServices : IAutowiredConfigureServices
    {
        public void ConfigureServices(IKernelServices kernelService)
        {
            kernelService.AddEfEntityMappings();
            var dbContextTypes = AssemblyHelper.GetFinalSubTypes<ShashlikDbContext>(kernelService.ScanFromDependencyContext);
            if (dbContextTypes.IsNullOrEmpty())
                return;

            foreach (var item in dbContextTypes)
            {
                kernelService.Services.TryAddScoped(
                    typeof(IEfNestedTransaction<>).MakeGenericType(item),
                    typeof(DefaultEfNestedTransaction<>).MakeGenericType(item)
                );
            }
        }
    }
}
