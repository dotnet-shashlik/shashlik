using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Exceptions;
using Shashlik.Utils.Extensions;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// ServiceTypeDescriptor provider
    /// </summary>
    public class DefaultServiceDescriptorProvider : IServiceDescriptorProvider
    {
        private static bool CanAddService(Type subType, Type parentType)
        {
            if (subType == parentType)
                return true;
            if (subType.IsGenericTypeDefinition != parentType.IsGenericTypeDefinition)
                return false;
            if (!subType.IsGenericTypeDefinition && !parentType.IsGenericTypeDefinition)
                return subType.IsSubTypeOrEqualsOf(parentType);
            return subType.IsSubTypeOfGenericDefinitionType(parentType);
        }

        /// <summary>
        /// 验证是否有冲突的service lifetime
        /// </summary>
        /// <param name="serviceLifetime"></param>
        /// <param name="type"></param>
        /// <exception cref="KernelServiceException"></exception>
        private static void ValidServiceLifetime(ServiceLifetime serviceLifetime, TypeInfo type)
        {
            var finalSubTypeLifeTimeAttribute = type.GetCustomAttribute<ServiceAttribute>(false);
            if (finalSubTypeLifeTimeAttribute != null
                && finalSubTypeLifeTimeAttribute.ServiceLifetime != serviceLifetime)
                throw new KernelServiceException(
                    $"Conflict service lifetime: {serviceLifetime}:{type.FullName} & {finalSubTypeLifeTimeAttribute.ServiceLifetime}:{type.FullName}");
        }

        /// <summary>
        /// 获取类所有的注册条件
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<ConditionDescriptor> GetConditions(Type type)
        {
            return type.GetCustomAttributes<ConditionBaseAttribute>(false)
                .Select(r =>
                    new ConditionDescriptor(r, r.GetType().GetCustomAttribute<OrderAttribute>(false)?.Order ?? int.MaxValue))
                .ToList();
        }


        private static bool IsIgnored(Type serviceType, Type[] ignores)
        {
            if (ignores.IsNullOrEmpty())
                return false;
            return ignores.Any(r =>
                r == serviceType || (r.IsGenericTypeDefinition && serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == r));
        }

        public IEnumerable<ShashlikServiceDescriptor> GetDescriptor(TypeInfo type)
        {
            var serviceAttribute = type.GetCustomAttribute<ServiceAttribute>(false);
            if (serviceAttribute is null)
                return new ShashlikServiceDescriptor[0];

            var res = new List<ShashlikServiceDescriptor>();
            var conditions = GetConditions(type);

            // 如果是抽象类或接口,只注册最终实现类和自身
            if (type.IsAbstract || type.IsInterface)
            {
                throw new KernelServiceException($"[Scoped]/[Singleton]/[Transient] can not used on type \"{type}\".");
            }

            var ignores = serviceAttribute.IgnoreServiceType;
            if (!IsIgnored(type, ignores))
            {
                // 注册自身
                var serviceDescriptor = ServiceDescriptor.Describe(type, type, serviceAttribute.ServiceLifetime);
                res.Add(new ShashlikServiceDescriptor(serviceDescriptor, conditions));
            }

            var baseTypes = type.GetAllInterfaces(false).ToHashSet();
            if (type.BaseType != typeof(object))
                baseTypes.Add(type.BaseType);
            foreach (var additionServiceType in serviceAttribute.AdditionServiceType)
            {
                if (type.IsSubType(additionServiceType))
                    baseTypes.Add(additionServiceType);
            }

            foreach (var baseType in baseTypes)
            {
                if (IsIgnored(baseType, ignores))
                    continue;

                ValidServiceLifetime(serviceAttribute.ServiceLifetime, baseType.GetTypeInfo());

                if (CanAddService(type, baseType))
                {
                    var serviceDescriptor = ServiceDescriptor.Describe(baseType, type, serviceAttribute.ServiceLifetime);
                    var currentConditions = conditions.Concat(GetConditions(baseType)).ToList();
                    res.Add(new ShashlikServiceDescriptor(serviceDescriptor, currentConditions));
                }
            }

            return res;
        }
    }
}