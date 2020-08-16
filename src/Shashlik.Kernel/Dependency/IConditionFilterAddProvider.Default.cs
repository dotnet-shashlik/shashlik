﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shashlik.Kernel.Dependency
{
    public class DefaultConditionFilterAddProvider : IConditionFilterAddProvider
    {
        public void FilterAdd(IEnumerable<ShashlikServiceDescriptor> serviceDescriptors, IServiceCollection services, IConfiguration rootConfiguration, IHostEnvironment hostEnvironment)
        {
            // 先全部注册一遍,再根据条件进行删除
            foreach (var item in serviceDescriptors)
                services.Add(item.ServiceDescriptor);

            foreach (var item in serviceDescriptors)
            {
                if (!item.Conditions.All(r => r.condition.ConditionOn(services, rootConfiguration, hostEnvironment)))
                    services.Remove(item.ServiceDescriptor);
            }

            // 查询有哪些条件序号
            var orders = serviceDescriptors.SelectMany(r => r.Conditions.Select(c => c.order)).Distinct();

            // 根据
            foreach (var order in orders)
            {
                foreach (var item in serviceDescriptors)
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
