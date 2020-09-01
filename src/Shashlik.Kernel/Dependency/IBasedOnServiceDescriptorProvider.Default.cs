using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 默认使用的约定服务查找器,只会注册接口以及自身为服务
    /// </summary>
    class DefaultBasedOnServiceDescriptorProvider : IBasedOnServiceDescriptorProvider
    {

        public IEnumerable<ShashlikServiceDescriptor> FromAssembly(Assembly assembly, TypeInfo baseType, ServiceLifetime serviceLifetime)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = AssemblyHelper.GetFinalSubTypes(baseType, assembly);

            List<ShashlikServiceDescriptor> result = new List<ShashlikServiceDescriptor>();
            types.ForEachItem(type =>
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

                        if (Utils.GenericArgumentsIsMatch(arg1, arg2))
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

                        if (Utils.GenericArgumentsIsMatch(arg1, arg2))
                            services.Add(baseType.GetGenericTypeDefinition());
                    }
                    else
                        services.Add(baseType);
                }

                services.Add(type);
                services.ForEach(service =>
                {
                    var serviceDescriptor = ServiceDescriptor.Describe(service, type, serviceLifetime);
                    result.Add(new ShashlikServiceDescriptor
                    {
                        ServiceDescriptor = serviceDescriptor,
                        Conditions = Utils.GetConditions(service)
                    });
                });
            });

            return result;
        }
    }
}
