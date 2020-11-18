using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace Shashlik.Kernel.Dependency
{
    public class DefaultFilterAddProvider : IFilterProvider
    {
        public void DoFilter(
            IServiceCollection services,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            var list = new List<ShashlikServiceDescriptor>();
            foreach (var serviceDescriptor in services)
            {
                if (serviceDescriptor is ShashlikServiceDescriptor s)
                    list.Add(s);
            }

            // 计算整个系统包含哪些条件序号
            var orders = list
                .SelectMany(r => r.Conditions.Select(c => c.Order))
                .Distinct()
                .OrderBy(r => r);

            // 根据条件排序号,从小到大依次过滤
            foreach (var order in orders)
            {
                foreach (var item in list)
                {
                    var condition = item.Conditions.FirstOrDefault(r => r.Order == order);
                    if (condition?.Condition is null)
                        continue;

                    if (!condition.Condition.ConditionOn(services, item, rootConfiguration, hostEnvironment))
                        // 不满足条件则移除该服务
                        services.Remove(item);
                }
            }
        }
    }
}