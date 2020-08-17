using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Autowire.Attributes;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    public class DefaultAutowireInitializer : IAutowireInitializer
    {
        public void Init(IDictionary<TypeInfo, AutowireDescriptor> autoServices, Action<AutowireDescriptor> initAction)
        {
            foreach (var item in autoServices)
                Invoke(item.Value as InnerAutowireDescriptor, autoServices, initAction);
        }

        public IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(TypeInfo baseType, IDictionary<TypeInfo, TypeInfo> replaces = null, IEnumerable<TypeInfo> removes = null, DependencyContext dependencyContext = null)
        {
            var types = AssemblyHelper.GetFinalSubTypes(baseType, dependencyContext);
            if (removes.IsNullOrEmpty())
                types = types.Except(removes).ToList();
            Dictionary<TypeInfo, AutowireDescriptor> descriptors = new Dictionary<TypeInfo, AutowireDescriptor>();

            foreach (var item in types)
            {
                var serviceType = replaces?.GetOrDefault(item) ?? item;
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afters = serviceType.GetCustomAttribute<AfterAttribute>()?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                var befores = serviceType.GetCustomAttribute<BeforeAttribute>()?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                descriptors.Add(serviceType, new InnerAutowireDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = InitStatus.Waiting
                });
            }

            foreach (var item in descriptors)
            {
                if (!item.Value.After.IsNullOrEmpty())
                {
                    foreach (var after in item.Value.After)
                    {
                        descriptors.GetOrDefault(after)?.DependsAfter?.Add(after);
                        item.Value.DependsBefore.Add(after);
                    }
                }

                if (!item.Value.Before.IsNullOrEmpty())
                {
                    foreach (var before in item.Value.Before)
                    {
                        descriptors.GetOrDefault(before)?.DependsBefore?.Add(before);
                        item.Value.DependsAfter.Add(before);
                    }
                }
            }

            return descriptors;
        }

        public IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(TypeInfo baseType, IServiceProvider serviceProvider, IDictionary<TypeInfo, TypeInfo> replaces = null, IEnumerable<TypeInfo> removes = null, DependencyContext dependencyContext = null)
        {
            var instances = serviceProvider.GetServices(baseType);
            Dictionary<TypeInfo, AutowireDescriptor> descriptors = new Dictionary<TypeInfo, AutowireDescriptor>();

            foreach (var item in instances)
            {
                var type = item.GetType().GetTypeInfo();
                if (!removes.IsNullOrEmpty() && removes.Contains(type))
                    continue;
                var serviceType = replaces?.GetOrDefault(type) ?? type;
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afters = serviceType.GetCustomAttribute<AfterAttribute>()?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                var befores = serviceType.GetCustomAttribute<BeforeAttribute>()?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                descriptors.Add(serviceType, new InnerAutowireDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = InitStatus.Waiting,
                    ServiceInstance = item
                });
            }

            foreach (var item in descriptors)
            {
                if (!item.Value.After.IsNullOrEmpty())
                {
                    foreach (var after in item.Value.After)
                    {
                        descriptors.GetOrDefault(after)?.DependsAfter?.Add(after);
                        item.Value.DependsBefore.Add(after);
                    }
                }

                if (!item.Value.Before.IsNullOrEmpty())
                {
                    foreach (var before in item.Value.Before)
                    {
                        descriptors.GetOrDefault(before)?.DependsBefore?.Add(before);
                        item.Value.DependsAfter.Add(before);
                    }
                }
            }

            return descriptors;
        }

        public IDictionary<TypeInfo, AutowireDescriptor> LoadFromAttribute(TypeInfo attributeType, DependencyContext dependencyContext = null, bool inherit = true)
        {
            return AssemblyHelper.GetTypesAndAttribute(attributeType, dependencyContext, inherit)
                .ToDictionary(r => r.Key, r => new InnerAutowireDescriptor
                {
                    ServiceInstance = r.Value,
                    Status = InitStatus.Waiting,
                    ServiceType = r.Key,
                } as AutowireDescriptor);
        }

        void Invoke(InnerAutowireDescriptor descriptor, IDictionary<TypeInfo, AutowireDescriptor> autoServices, Action<AutowireDescriptor> initAction)
        {
            if (descriptor.Status == InitStatus.Done)
                return;
            // 递归中发现挂起的服务那就是有循环依赖
            if (descriptor.Status == InitStatus.Hangup)
                throw new System.Exception($"exists circular dependencies on {descriptor.ServiceType}.");

            // 在这个类型之前已经没有依赖了
            if (descriptor.DependsBefore.IsNullOrEmpty())
            {
                initAction(descriptor);
                descriptor.Status = InitStatus.Done;
            }
            else
            {
                descriptor.Status = InitStatus.Hangup;
                foreach (var item in descriptor.DependsBefore)
                {
                    Invoke(autoServices[item] as InnerAutowireDescriptor, autoServices, initAction);
                }
                descriptor.Status = InitStatus.Done;
            }
        }
    }
}
