using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Dependency
{
    public class ShashlikServiceDescriptor
    {
        /// <summary>
        /// 注册条件集合
        /// </summary>
        public List<(IConditionBase condition, int order)> Conditions { get; set; }

        /// <summary>
        /// 服务描述
        /// </summary>
        public ServiceDescriptor ServiceDescriptor { get; set; }
    }
}
