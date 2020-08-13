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
    class BasedOnServiceDescriptorProvider
    {
        public IEnumerable<ServiceDescriptor> FromAssembly(Assembly assembly, TypeInfo baseType, ServiceLifetime serviceLifetime)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly
                            .DefinedTypes
                            .Where(r => !r.IsAbstract && r.IsClass && r.BaseType != null)
                            .Where(r => r.IsChildTypeOf(baseType) || r.IsChildTypeOfGenericType(baseType))
                            ;

            List<ServiceDescriptor> result = new List<ServiceDescriptor>();
            types.Foreach(type =>
            {
                var arg1 = type.GetGenericArguments();

                List<Type> services = new List<Type>();
                foreach (var interfaceType in type.ImplementedInterfaces)
                {
                    if (interfaceType.Equals(type))
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
    }
}
