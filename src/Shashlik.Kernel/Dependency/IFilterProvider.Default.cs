using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shashlik.Kernel.Dependency
{
    public class DefaultFilterAddProvider : IFilterProvider
    {
        public void DoFilter(
            IEnumerable<ShashlikServiceDescriptor> serviceDescriptors,
            IServiceCollection services,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var list = serviceDescriptors.ToList();

            // 查询有哪些条件序号
            var orders = list
                .SelectMany(r => r.Conditions.Select(c => c.order))
                .Distinct()
                .OrderBy(r => r);

            // 根据条件排序号,从小到大依次过滤
            foreach (var order in orders)
            {
                foreach (var item in list)
                {
                    var condition = item.Conditions.FirstOrDefault(r => r.order == order);
                    if (condition.condition is null)
                        continue;

                    if (!condition.condition.ConditionOn(services, item.ServiceDescriptor, rootConfiguration,
                        hostEnvironment))
                    {
                        services.RemoveByImplType(item.ServiceDescriptor.ImplementationType);
                        services.RemoveByServiceType(item.ServiceDescriptor.ImplementationType);
                    }
                }
            }
        }
    }
}