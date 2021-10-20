using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shashlik.Kernel.Dependency
{
    public abstract class ServiceAttribute : Attribute
    {
        protected ServiceAttribute(ServiceLifetime serviceLifetime, Type[] additionServiceType, Type[] ignoreServiceType)
        {
            ServiceLifetime = serviceLifetime;
            AdditionServiceType = additionServiceType ?? throw new ArgumentNullException(nameof(additionServiceType));
            IgnoreServiceType = ignoreServiceType;
        }

        /// <summary>
        /// 服务生命周期
        /// </summary>
        public ServiceLifetime ServiceLifetime { get; }

        /// <summary>
        /// 附加服务类
        /// </summary>
        public Type[] AdditionServiceType { get; }

        /// <summary>
        /// 需要忽略的服务类型
        /// </summary>
        public Type[] IgnoreServiceType { get; }
    }
}