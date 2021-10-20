using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件依赖,服务存在时,优先级200, 自动装配类IServiceAssembler无效
    /// </summary>
    [Order(200)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ConditionDependsOnAttribute : ConditionBaseAttribute
    {
        /// <summary>
        /// 条件依赖,服务存在时,优先级200
        /// </summary>
        /// <param name="types">依赖的服务类型</param>
        public ConditionDependsOnAttribute(params Type[] types)
        {
            Types = types;
        }

        /// <summary>
        /// 依赖的服务类型
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
                    return Types.All(services.AnyService);
                case ConditionType.ANY:
                    return Types.Any(services.AnyService);
                default: throw new IndexOutOfRangeException($"Error condition type {ConditionType} on {serviceDescriptor.ImplementationType}");
            }
        }
    }
}