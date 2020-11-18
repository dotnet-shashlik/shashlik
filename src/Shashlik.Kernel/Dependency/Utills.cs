using Microsoft.Extensions.DependencyInjection;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Kernel.Dependency
{
    public static class Utils
    {
        /// <summary>
        /// 获取类所有的注册条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ConditionDescriptor> GetConditions(Type type)
        {
            return type.GetCustomAttributes<ConditionBaseAttribute>(false)
                .Select(r =>
                    new ConditionDescriptor(r,
                        r.GetType().GetCustomAttribute<OrderAttribute>(false)?.Order ?? int.MaxValue))
                .ToList();
        }

        /// <summary>
        /// 服务集合是否已经存在<typeparamref name="TType"/>服务类型
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static bool AnyService<TType>(this IServiceCollection services)
        {
            return services.AnyService(typeof(TType));
        }

        /// <summary>
        /// 服务集合是否已经存在<paramref name="serviceType"/>服务类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public static bool AnyService(this IServiceCollection services, Type serviceType)
        {
            return services.Any(r => r.ServiceType == serviceType);
        }
    }
}