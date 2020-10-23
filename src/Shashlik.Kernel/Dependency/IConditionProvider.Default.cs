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
    public class DefaultConditionFilterAddProvider : IConditionProvider
    {
        public void FilterAndRegistryService(IEnumerable<ShashlikServiceDescriptor> serviceDescriptors, IServiceCollection services,
            IConfiguration rootConfiguration, IHostEnvironment hostEnvironment)
        {
            // 先全部注册一遍,再根据条件进行删除
            var list = serviceDescriptors.ToList();
            foreach (var item in list)
                services.Add(item.ServiceDescriptor);
 
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
                    if (condition.condition == null)
                        continue;

                    if (!condition.condition.ConditionOn(services, rootConfiguration, hostEnvironment))
                        services.Remove(item.ServiceDescriptor);
                }
            }
        }
    }
}