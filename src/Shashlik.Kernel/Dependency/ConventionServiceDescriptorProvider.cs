using Guc.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Guc.Kernel.Dependency
{
    /// <summary>
    /// 默认使用的约定服务查找器,只会注册接口以及自身为服务
    /// </summary>
    class ConventionServiceDescriptorProvider
    {
        public IEnumerable<ServiceDescriptor> FromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly
                            .DefinedTypes
                            .Where(r => !r.IsAbstract && r.IsClass && r.BaseType != null)
                            .Where(r => !r.ImplementedInterfaces.IsNullOrEmpty());

            List<ServiceDescriptor> result = new List<ServiceDescriptor>();
            types.Foreach(type =>
            {
                ValidInterfaces(type);

                ServiceLifetime serviceLifetime;

                if (type.IsChildTypeOf<ITransient>())
                    serviceLifetime = ServiceLifetime.Transient;
                else if (type.IsChildTypeOf<ISingleton>())
                    serviceLifetime = ServiceLifetime.Singleton;
                else if (type.IsChildTypeOf<IScoped>())
                    serviceLifetime = ServiceLifetime.Scoped;
                else
                    return;

                var arg1 = type.GetGenericArguments();

                List<Type> services = new List<Type>();
                foreach (var interfaceType in type.ImplementedInterfaces)
                {
                    if (IsConvectionInterfaceType(interfaceType))
                        continue;

                    if (type.IsGenericTypeDefinition)
                    {
                        var arg2 = interfaceType.GetGenericArguments();

                        if (GenericArgumentsIsMatch(arg1, arg2))
                            services.Add(interfaceType.GetGenericTypeDefinition());
                    }
                    else
                        services.Add(interfaceType);
                }

                foreach (var baseType in type.GetAllBaseTypes())
                {
                    if (type.IsGenericTypeDefinition)
                    {
                        var arg2 = baseType.GetGenericArguments();

                        if (GenericArgumentsIsMatch(arg1, arg2))
                            services.Add(baseType.GetGenericTypeDefinition());
                    }
                    else
                        services.Add(baseType);
                }

                services.Add(type);
                services.ForEach(service =>
                {
                    result.Add(ServiceDescriptor.Describe(service, type, serviceLifetime));
                });
            });

            return result;
        }

        /// <summary>
        /// 泛型参数是否匹配
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        bool GenericArgumentsIsMatch(Type[] arg1, Type[] arg2)
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
        bool IsConvectionInterfaceType(Type type)
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
        void ValidInterfaces(TypeInfo type)
        {
            var convectionInterfaces = type.ImplementedInterfaces.Where(r => IsConvectionInterfaceType(r)).ToList();
            if (convectionInterfaces.Count > 1)
                throw new System.Exception($"convention type:{type} can't inherit from multiple interface:{Environment.NewLine}{convectionInterfaces.Select(r => r.FullName).Join(Environment.NewLine)}");
        }
    }
}
