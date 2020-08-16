using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Kernel.Automatic.Attributes;
using Shashlik.Utils.Common;
using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Automatic
{
    public class DefaultAutoInitializer : IAutoInitializer
    {
        public void Init(IDictionary<TypeInfo, AutoDescriptor> autoServices, Action<AutoDescriptor> initAction)
        {
            foreach (var item in autoServices)
                Invoke(item.Value, autoServices, initAction);
        }

        public IDictionary<TypeInfo, AutoDescriptor> Scan<TBaseType>(IDictionary<TypeInfo, TypeInfo> replaces = null, DependencyContext dependencyContext = null)
        {
            var types = AssemblyHelper.GetFinalSubTypes<TBaseType>(dependencyContext);
            Dictionary<TypeInfo, AutoDescriptor> descriptors = new Dictionary<TypeInfo, AutoDescriptor>();

            foreach (var item in types)
            {
                var serviceType = replaces?.GetOrDefault(item) ?? item;
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afters = serviceType.GetCustomAttribute<AfterAttribute>()?.Types?.Where(r => r.IsChildTypeOf<TBaseType>())?.ToArray();
                var befores = serviceType.GetCustomAttribute<BeforeAttribute>()?.Types?.Where(r => r.IsChildTypeOf<TBaseType>())?.ToArray();
                descriptors.Add(serviceType, new AutoDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = AutoDescriptor._Status.Waiting
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

        public IDictionary<TypeInfo, AutoDescriptor> Scan<TBaseType>(IServiceProvider serviceProvider, IDictionary<TypeInfo, TypeInfo> replaces = null, DependencyContext dependencyContext = null)
        {
            var instances = serviceProvider.GetServices<TBaseType>();
            Dictionary<TypeInfo, AutoDescriptor> descriptors = new Dictionary<TypeInfo, AutoDescriptor>();

            foreach (var item in instances)
            {
                var type = item.GetType().GetTypeInfo();
                var serviceType = replaces?.GetOrDefault(type) ?? type;
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afters = serviceType.GetCustomAttribute<AfterAttribute>()?.Types?.Where(r => r.IsChildTypeOf<TBaseType>())?.ToArray();
                var befores = serviceType.GetCustomAttribute<BeforeAttribute>()?.Types?.Where(r => r.IsChildTypeOf<TBaseType>())?.ToArray();
                descriptors.Add(serviceType, new AutoDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = AutoDescriptor._Status.Waiting,
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


        public IDictionary<TypeInfo, TAttribute> ScanAttribute<TAttribute>(DependencyContext dependencyContext = null, bool inherit = true)
            where TAttribute : Attribute
        {
            return AssemblyHelper.GetTypesByAttribute<TAttribute>(dependencyContext, inherit);
        }

        void Invoke(AutoDescriptor descriptor, IDictionary<TypeInfo, AutoDescriptor> autoServices, Action<AutoDescriptor> initAction)
        {
            if (descriptor.Status == AutoDescriptor._Status.Done)
                return;
            // 递归中发现挂起的服务那就是有循环依赖
            if (descriptor.Status == AutoDescriptor._Status.Hangup)
                throw new System.Exception($"exists circular dependencies on {descriptor.ServiceType}.");

            // 在这个类型之前已经没有依赖了
            if (descriptor.DependsBefore.IsNullOrEmpty())
            {
                initAction(descriptor);
                descriptor.Status = AutoDescriptor._Status.Done;
            }
            else
            {
                descriptor.Status = AutoDescriptor._Status.Hangup;
                foreach (var item in descriptor.DependsBefore)
                {
                    Invoke(autoServices[item], autoServices, initAction);
                }
                descriptor.Status = AutoDescriptor._Status.Done;
            }
        }
    }
}
