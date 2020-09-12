﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Autowire.Attributes;
using Shashlik.Kernel.Autowired.Inner;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Kernel.Autowired
{
    public class DefaultAutowiredProvider : IAutowiredProvider
    {
        public void Autowired(IDictionary<TypeInfo, AutowiredDescriptor> autoServices,
            Action<AutowiredDescriptor> autowiredAction)
        {
            foreach (var item in autoServices
                    .OrderBy(r => r.Value.Prevs.IsNullOrEmpty() && r.Value.Nexts.IsNullOrEmpty() ? 0 : 1))
                // 把没有任务顺序要求的放在最前面执行
                Invoke(item.Value as InnerAutowiredDescriptor, autoServices, autowiredAction);
        }

        public IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceCollection services,
            DependencyContext dependencyContext = null)
        {
            var types = AssemblyHelper.GetFinalSubTypes(baseType, dependencyContext);
            var descriptors = new Dictionary<TypeInfo, AutowiredDescriptor>();

            foreach (var serviceType in types)
            {
                var afterOnAttribute = serviceType.GetCustomAttribute<AfterAtAttribute>();
                var beforeOnAttribute = serviceType.GetCustomAttribute<BeforeAtAttribute>();
                if (beforeOnAttribute != null && afterOnAttribute != null)
                    throw new KernelExceptionInitException($"[AfterAt] and [BeforeAt] cannot be used together.");

                //bool isRemove = !removes.IsNullOrEmpty() && removes.Contains(serviceType);
                services.AddSingleton(serviceType, serviceType);
                descriptors.Add(serviceType, new InnerAutowiredDescriptor(
                    FilterType(afterOnAttribute?.AfterAt, baseType),
                    FilterType(beforeOnAttribute?.BeforeAt, baseType), serviceType, InitStatus.Waiting)
                );
            }

            using var serviceProvider = services.BuildServiceProvider();

            foreach (var item in descriptors)
            {
                if (item.Value.AfterAt != null)
                {
                    item.Value.Prevs.Add(item.Value.AfterAt);
                    descriptors[item.Value.AfterAt].Nexts.Add(item.Key);
                }

                if (item.Value.BeforeAt != null)
                {
                    item.Value.Nexts.Add(item.Value.BeforeAt);
                    descriptors[item.Value.BeforeAt].Prevs.Add(item.Key);
                }

                item.Value.ServiceInstance = serviceProvider.GetService(item.Key);
            }

            return descriptors;
        }

        public IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceProvider serviceProvider)
        {
            var instances = serviceProvider.GetServices(baseType);
            var descriptors = new Dictionary<TypeInfo, AutowiredDescriptor>();
            foreach (var item in instances)
            {
                var serviceType = item.GetType().GetTypeInfo();
                if (descriptors.ContainsKey(serviceType))
                    continue;
                var afterOnAttribute = serviceType.GetCustomAttribute<AfterAtAttribute>();
                var beforeOnAttribute = serviceType.GetCustomAttribute<BeforeAtAttribute>();

                descriptors.Add(serviceType, new InnerAutowiredDescriptor(
                    FilterType(afterOnAttribute?.AfterAt, baseType),
                    FilterType(beforeOnAttribute?.BeforeAt, baseType), serviceType, InitStatus.Waiting)
                {
                    ServiceInstance = item
                });
            }

            foreach (var item in descriptors)
            {
                if (item.Value.AfterAt != null)
                {
                    item.Value.Prevs.Add(item.Value.AfterAt);
                    descriptors[item.Value.AfterAt].Nexts.Add(item.Key);
                }

                if (item.Value.BeforeAt != null)
                {
                    item.Value.Nexts.Add(item.Value.BeforeAt);
                    descriptors[item.Value.BeforeAt].Prevs.Add(item.Key);
                }

                item.Value.ServiceInstance = serviceProvider.GetService(item.Key);
            }

            return descriptors;
        }

        public IDictionary<TypeInfo, AutowiredDescriptor> LoadFrom(TypeInfo attributeType,
            DependencyContext dependencyContext = null, bool inherit = true)
        {
            return AssemblyHelper.GetTypesAndAttribute(attributeType, dependencyContext, inherit)
                .ToDictionary(r => r.Key,
                    r => new InnerAutowiredDescriptor(null, null, r.Key, InitStatus.Waiting)
                    {
                        Attribute = r.Value,
                    } as AutowiredDescriptor);
        }

        private void Invoke(InnerAutowiredDescriptor descriptor,
            IDictionary<TypeInfo, AutowiredDescriptor> autoServices,
            Action<AutowiredDescriptor> initAction)
        {
            if (descriptor.Status == InitStatus.Done)
                return;
            // 递归中发现挂起的服务那就是有循环依赖
            if (descriptor.Status == InitStatus.Hangup)
                throw new System.Exception($"exists circular dependencies on {descriptor.ServiceType}.");

            // 在这个类型之前已经没有依赖了
            if (descriptor.Prevs.IsNullOrEmpty())
            {
                initAction(descriptor);
                descriptor.Status = InitStatus.Done;
            }
            else
            {
                descriptor.Status = InitStatus.Hangup;
                foreach (var item in descriptor.Prevs)
                    Invoke(autoServices[item] as InnerAutowiredDescriptor, autoServices, initAction);

                descriptor.Status = InitStatus.Done;
            }
        }

        private TypeInfo? FilterType(Type? type, Type baseType)
        {
            if (type == null)
                return null;
            return type.IsSubTypeOf(baseType) ? type.GetTypeInfo() : null;
        }
    }
}