using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Shashlik.Kernel.Dependency
{
    internal static class Utils
    {
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

        /// <summary>
        /// 服务集合是否已经存在<paramref name="serviceType"/>服务类型
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceType"></param>
        /// <param name="implType"></param>
        /// <returns></returns>
        public static bool AnyService(this IServiceCollection services, Type serviceType, Type implType)
        {
            return services.Any(r => r.ServiceType == serviceType && r.ImplementationType == implType);
        }
    }
}