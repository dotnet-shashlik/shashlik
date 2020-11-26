using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件依赖,服务不存在时,优先级100, 自动装配类IServiceAutowire无效
    /// </summary>
    [Order(100)]
    public class ConditionDependsOnMissingAttribute : ConditionBaseAttribute
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

        public override bool ConditionOn(
            IServiceCollection services,
            ServiceDescriptor serviceDescriptor,
            IConfiguration rootConfiguration,
            IHostEnvironment hostEnvironment)
        {
            switch (ConditionType)
            {
                case ConditionType.ALL:
                    return Types.All(r => !services.Any(s =>
                        s.ServiceType == r && s.ImplementationType != serviceDescriptor.ImplementationType));
                case ConditionType.ANY:
                    return Types.Any(r => !services.Any(s =>
                        s.ServiceType == r && s.ImplementationType != serviceDescriptor.ImplementationType));
                default: throw new IndexOutOfRangeException($"Error condition type {ConditionType} on {serviceDescriptor.ImplementationType}");
            }
        }
    }
}