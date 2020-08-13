using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Guc.Kernel.Dependency
{
    public static class Extensions
    {
        /// <summary>
        /// 注册程序集中约定的服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static void AddServiceByConvention(this IServiceCollection services, Assembly assembly)
        {
            new ConventionServiceDescriptorProvider()
                .FromAssembly(assembly)
                .Foreach(services.Add);
        }

        /// <summary>
        /// 注册程序集中约定的服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly">程序集</param>
        public static void AddServiceByBasedOn(this IServiceCollection services, TypeInfo type, Assembly assembly, ServiceLifetime serviceLifetime)
        {
            new BasedOnServiceDescriptorProvider()
                .FromAssembly(assembly, type, serviceLifetime)
                .Foreach(services.Add);
        }

        /// <summary>
        /// 循环集合元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="action"></param>
        internal static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var item in list)
            {
                action(item);
            }
        }

        /// <summary>
        /// 判断集合是否为null或者空
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || !list.Any();
        }

        /// <summary>
        /// 是否为<paramref name="parentType"/>的子类
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType"></param>
        /// <returns></returns>
        internal static bool IsChildTypeOf(this Type type, Type parentType)
        {
            return parentType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否为<typeparamref name="T"/>的子类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsChildTypeOf<T>(this Type type)
        {
            return type.IsChildTypeOf(typeof(T));
        }

        private static void fillBaseType(HashSet<Type> results, Type type)
        {
            if (type.BaseType != typeof(object))
            {
                results.Add(type.BaseType);
                fillBaseType(results, type.BaseType);
            }
        }

        /// <summary>
        /// 获取所有的父级类型,不包含接口和object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static HashSet<Type> GetAllBaseTypes(this Type type)
        {
            HashSet<Type> types = new HashSet<Type>();
            fillBaseType(types, type);
            return types;
        }

    }
}
