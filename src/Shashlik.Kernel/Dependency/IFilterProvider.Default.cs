﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;

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

            var latest = new List<ShashlikServiceDescriptor>();

            // 根据条件排序号,从小到大依次过滤
            foreach (var order in orders)
            {
                foreach (var item in list)
                {
                    item.Conditions
                        .Where(r => r.Order == order)
                        .ForEachItem(condition =>
                        {
                            if (!condition.Condition.ConditionOn(services, item, rootConfiguration, hostEnvironment))
                                // 不满足条件则移除该服务
                                services.Remove(item);
                        });

                    if (item.ImplementationType!.IsDefinedAttribute<LatestImplementationAttribute>(true))
                    {
                        services.Remove(item);
                        latest.Add(item);
                    }
                }
            }

            latest.ForEachItem(services.Add);
        }
    }
}