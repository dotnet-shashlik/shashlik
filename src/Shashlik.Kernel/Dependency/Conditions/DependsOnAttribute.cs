using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shashlik.Kernel.Dependency.Conditions
{
    /// <summary>
    /// 条件依赖,服务存在时
    /// </summary>
    [ConditionOrder(200)]
    public class ConditionDependsOnAttribute : Attribute, IConditionBase
    {
        /// <summary>
        /// 条件依赖,服务存在时
        /// </summary>
        /// <param name="types">依赖的服务类型</param>
        public ConditionDependsOnAttribute(params Type[] types)
        {
            Types = types;
        }

        /// <summary>
        /// 依赖的服务类型
        /// </summary>
        public Type[] Types { get; set; }

        /// <summary>
        /// 默认为ALL
        /// </summary>
        public ConditionType ConditionType { get; set; } = ConditionType.ALL;


        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration, IHostEnvironment hostEnvironment)
        {
            switch (ConditionType)
            {
                case ConditionType.ALL:
                    return Types.All(r => services.AnyService(r));
                case ConditionType.ANY:
                    return Types.Any(r => services.AnyService(r));
                default: throw new Exception($"error condition type: {ConditionType}");
            }
        }
    }
}
