using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Dependency.Conditions;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Dependency
{
    public static class Utils
    {
        /// <summary>
        /// 泛型参数是否匹配
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static bool GenericArgumentsIsMatch(Type[] arg1, Type[] arg2)
        {
            if (arg1 == null || arg1.Length == 0)
                return false;

            if (arg2 == null || arg2.Length == 0)
                return false;

            if (arg1.Length != arg2.Length)
                return false;

            for (int i = 0; i < arg1.Length; i++)
            {
                if (arg1[i] != arg2[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 是否为约定的接口类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsConvectionInterfaceType(Type type)
        {
            if (type == typeof(ITransient) ||
                type == typeof(ISingleton) ||
                type == typeof(IScoped))
                return true;
            return false;
        }


        /// <summary>
        /// 验证接口继承
        /// </summary>
        /// <param name="type"></param>
        public static void ValidInterfaces(TypeInfo type)
        {
            var convectionInterfaces = type.ImplementedInterfaces.Where(r => IsConvectionInterfaceType(r)).ToList();
            if (convectionInterfaces.Count > 1)
                throw new System.Exception(
                    $"convention type:{type} can't inherit from multiple interface:{Environment.NewLine}{convectionInterfaces.Select(r => r.FullName).Join(Environment.NewLine)}");
        }

        /// <summary>
        /// 获取类所有的注册条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<(IConditionBase condition, int order)> GetConditions(Type type)
        {
            return type.GetCustomAttributes()
                .Where(r => r is IConditionBase)
                .Select(r => (r as IConditionBase, r.GetType().GetCustomAttribute<ConditionOrderAttribute>().Order))
                .ToList();
        }

        private static void FillBaseType(HashSet<Type> results, Type type)
        {
            if (type.BaseType != typeof(object))
            {
                results.Add(type.BaseType);
                FillBaseType(results, type.BaseType);
            }
        }

        /// <summary>
        /// 获取所有的父级类型,不包含接口和object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HashSet<Type> GetAllBaseTypes(this Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            FillBaseType(types, type);
            return types;
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