using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Dependency
{
    /// <summary>
    /// ServiceTypeDescriptor provider
    /// </summary>
    public class DefaultServiceDescriptorProvider : IServiceDescriptorProvider
    {
        public DefaultServiceDescriptorProvider(IKernelServices kernelServices)
        {
            KernelServices = kernelServices;
        }

        private IKernelServices KernelServices { get; }

        /// <summary>
        /// 是不是没有子类的最终类
        /// </summary>
        /// <param name="types"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsLastFinalType(IEnumerable<TypeInfo> types, Type type)
        {
            foreach (var typeInfo in types)
            {
                if (typeInfo == type)
                    continue;
                if (typeInfo.IsSubTypeOrEqualsOf(type))
                    return false;
                if (typeInfo.IsSubTypeOfGenericType(type))
                    return false;
            }

            return true;
        }

        private static bool CanAddService(Type subType, Type parentType, out List<Type> serviceTypeList)
        {
            serviceTypeList = new List<Type>() {parentType};
            if (subType == parentType)
                return true;
            if (!subType.IsGenericTypeDefinition && parentType.IsGenericTypeDefinition)
            {
                // 子类不是泛型定义,父类是,注册父类的泛型实现类为服务类
                serviceTypeList = subType.GetAllBaseTypesAndInterfaces()
                    .Where(r => !r.IsGenericTypeDefinition && r.IsSubTypeOfGenericType(parentType))
                    .ToList();

                if (serviceTypeList.IsNullOrEmpty())
                    return false;
                return true;
            }

            if (subType.IsGenericTypeDefinition != parentType.IsGenericTypeDefinition)
                return false;
            if (!subType.IsGenericTypeDefinition && !parentType.IsGenericTypeDefinition)
                return subType.IsSubTypeOrEqualsOf(parentType);
            return subType.IsSubTypeOfGenericDefinitionType(parentType);
        }

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

        /// <summary>
        /// 获取继承链所有的ignores
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static HashSet<Type> GetIgnoresFromInheritedChain(Type type)
        {
            List<Type> res = new List<Type>();
            foreach (var item in type.GetAllBaseTypesAndInterfaces())
            {
                var serviceAttribute = item.GetCustomAttribute<ServiceAttribute>();
                if (serviceAttribute == null)
                    continue;
                res.AddRange(serviceAttribute.IgnoreServices);
            }

            return res.ToHashSet();
        }

        public IEnumerable<ShashlikServiceDescriptor> GetDescriptor(TypeInfo type)
        {
            var serviceAttribute = type.GetCustomAttribute<ServiceAttribute>(false);
            if (serviceAttribute is null)
                return new ShashlikServiceDescriptor[0];

            var res = new List<ShashlikServiceDescriptor>();
            var conditions = GetConditions(type);

            // 注册整个继承链
            if (serviceAttribute.RequireRegistryInheritedChain)
            {
                var baseTypes = type.GetAllBaseTypesAndInterfaces().ToList();
                var subTypes = ReflectionHelper.GetSubTypes(type);
                var finalSubTypes = subTypes.Where(r => r.IsClass && !r.IsAbstract).ToList();
                var allTypes = baseTypes.Concat(subTypes).ToList();

                foreach (var item1 in finalSubTypes)
                {
                    var ignores = GetIgnoresFromInheritedChain(item1);
                    foreach (var item2 in allTypes)
                    {
                        if (ignores.Contains(item2))
                            continue;

                        if (!CanAddService(item1, item2, out var serviceTypeList))
                            continue;

                        foreach (var serviceType in serviceTypeList)
                        {
                            var serviceDescriptor = ServiceDescriptor.Describe(serviceType, item1, serviceAttribute.ServiceLifetime);
                            var conditions1 = GetConditions(item1);
                            var conditions2 = GetConditions(serviceType);
                            res.Add(new ShashlikServiceDescriptor(serviceDescriptor, conditions1.Concat(conditions2).ToList()));
                        }
                    }
                }
            }
            // 如果是抽象类或接口,只注册最终实现类和自身
            else if (type.IsAbstract || type.IsInterface)
            {
                var finalSubTypes = ReflectionHelper.GetFinalSubTypes(type, KernelServices.ScanFromDependencyContext);

                foreach (var finalSubType in finalSubTypes)
                {
                    if (GetIgnoresFromInheritedChain(finalSubType).Contains(finalSubType))
                        continue;
                    ValidServiceLifetime(serviceAttribute.ServiceLifetime, finalSubType);

                    // 只注册最下面的没有子类的最终类
                    if (IsLastFinalType(finalSubTypes, finalSubType))
                    {
                        var currentConditions = conditions.Concat(GetConditions(finalSubType)).ToList();
                        ServiceDescriptor self = ServiceDescriptor.Describe(finalSubType, finalSubType, serviceAttribute.ServiceLifetime);
                        res.Add(new ShashlikServiceDescriptor(self, currentConditions));
                        if (CanAddService(finalSubType, type, out var serviceTypeList))
                        {
                            foreach (var serviceType in serviceTypeList)
                            {
                                ServiceDescriptor @base = ServiceDescriptor.Describe(serviceType, finalSubType, serviceAttribute.ServiceLifetime);
                                // 每一个服务都使用服务类和实现的条件并集
                                res.Add(new ShashlikServiceDescriptor(@base, currentConditions));
                            }
                        }
                    }
                }
            }
            // 最终实现类，往上查找服务
            else if (type.IsClass && !type.IsAbstract)
            {
                var ignores = GetIgnoresFromInheritedChain(type);
                if (!ignores.Contains(type))
                {
                    // 注册自身
                    var serviceDescriptor = ServiceDescriptor.Describe(type, type, serviceAttribute.ServiceLifetime);
                    res.Add(new ShashlikServiceDescriptor(serviceDescriptor, conditions));
                }

                var baseTypes = type.GetAllBaseTypesAndInterfaces();
                foreach (var baseType in baseTypes)
                {
                    if (ignores.Contains(baseType))
                        continue;

                    ValidServiceLifetime(serviceAttribute.ServiceLifetime, baseType.GetTypeInfo());

                    if (CanAddService(type, baseType))
                    {
                        var serviceDescriptor = ServiceDescriptor.Describe(baseType, type, serviceAttribute.ServiceLifetime);
                        var currentConditions = conditions.Concat(GetConditions(baseType)).ToList();
                        res.Add(new ShashlikServiceDescriptor(serviceDescriptor, currentConditions));
                    }
                }
            }

            return res;
        }
    }
}