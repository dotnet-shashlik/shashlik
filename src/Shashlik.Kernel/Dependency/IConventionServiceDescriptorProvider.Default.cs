using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// 默认使用的约定服务查找器,只会注册接口以及自身为服务
    /// </summary>
    internal class DefaultConventionServiceDescriptorProvider : IConventionServiceDescriptorProvider
    {
        public IEnumerable<ShashlikServiceDescriptor> FromAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var types = assembly
                            .DefinedTypes
                            .Where(r => !r.IsAbstract && r.IsClass && r.BaseType != null)
                            .Where(r => !r.ImplementedInterfaces.IsNullOrEmpty());

            List<ShashlikServiceDescriptor> result = new List<ShashlikServiceDescriptor>();
            types.ForEachItem(type =>
            {
                Utils.ValidInterfaces(type);

                ServiceLifetime serviceLifetime;

                if (type.IsSubTypeOf<ITransient>())
                    serviceLifetime = ServiceLifetime.Transient;
                else if (type.IsSubTypeOf<ISingleton>())
                    serviceLifetime = ServiceLifetime.Singleton;
                else if (type.IsSubTypeOf<IScoped>())
                    serviceLifetime = ServiceLifetime.Scoped;
                else
                    return;

                var arg1 = type.GetGenericArguments();

                List<Type> services = new List<Type>();
                foreach (var interfaceType in type.ImplementedInterfaces)
                {
                    if (Utils.IsConvectionInterfaceType(interfaceType))
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
