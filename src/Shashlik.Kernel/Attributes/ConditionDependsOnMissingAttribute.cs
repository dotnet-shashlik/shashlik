using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件依赖,服务不存在时,优先级100
    /// </summary>
    [Order(100)]
    public class ConditionDependsOnMissingAttribute : Attribute, IConditionBase
    {
        /// <summary>
        /// 条件依赖,服务不存在时,优先级100
        /// </summary>
        /// <param name="types">依赖的服务类型</param>
        public ConditionDependsOnMissingAttribute(params Type[] types)
        {
            Types = types;
        }

        /// <summary>
        /// 依赖不存在的服务类型
        /// </summary>
        public Type[] Types { get; }

        /// <summary>
        /// 默认为ALL
        /// </summary>
        public ConditionType ConditionType { get; set; } = ConditionType.ALL;

        public bool ConditionOn(IServiceCollection services, IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            switch (ConditionType)
            {
                case ConditionType.ALL:
                    return Types.All(r => !services.AnyService(r));
                case ConditionType.ANY:
                    return Types.Any(r => !services.AnyService(r));
                default: throw new Exception($"error condition type: {ConditionType}");
            }
        }
    }
}