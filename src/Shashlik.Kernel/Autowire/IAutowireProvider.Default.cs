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
    public class DefaultAutowireProvider : IAutowireProvider
    {
        public void Autowire(IDictionary<TypeInfo, AutowireDescriptor> pipelineService, Action<AutowireDescriptor> autowireAction)
        {
            foreach (var item in pipelineService)
                Invoke(item.Value as InnerAutowireDescriptor, pipelineService, autowireAction);
        }

        public IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceCollection services,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null)
        {
            var types = AssemblyHelper.GetFinalSubTypes(baseType, dependencyContext);
            if (removes.IsNullOrEmpty()) types = types.Except(removes).ToList();
            Dictionary<TypeInfo, AutowireDescriptor> descriptors = new Dictionary<TypeInfo, AutowireDescriptor>();

            foreach (var serviceType in types)
            {
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afterOnAttribute = serviceType.GetCustomAttribute<AfterOnAttribute>();
                var afters = afterOnAttribute?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                var beforeOnAttribute = serviceType.GetCustomAttribute<BeforeOnAttribute>();
                var befores = beforeOnAttribute?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();

                services.AddSingleton(serviceType);
                descriptors.Add(serviceType, new InnerAutowireDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = InitStatus.Waiting,
                    AfterCancelOn = afterOnAttribute?.CancelOn
                });
            }

            // 根据AfterOn依赖规则,去掉不需要装载的类型
            List<TypeInfo> nomatchs = new List<TypeInfo>();
            foreach (var item in descriptors)
            {
                if (!item.Value.After.IsNullOrEmpty())
                {
                    if (item.Value.AfterCancelOn == CancelOn.AllNotExists
                        && !item.Value.After.All(r => descriptors.ContainsKey(r)))
                    {
                        nomatchs.Add(item.Key);
                    }
                    else if (item.Value.AfterCancelOn == CancelOn.AnyNotExists
                        && item.Value.After.Any(r => !descriptors.ContainsKey(r)))
                    {
                        nomatchs.Add(item.Key);
                    }
                }
            }
            foreach (var item in nomatchs)
                descriptors.Remove(item);

            using var serviceProvider = services.BuildServiceProvider();
            foreach (var item in descriptors)
            {
                // 注入实例
                item.Value.ServiceInstance = serviceProvider.GetService(item.Key);

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

        public IDictionary<TypeInfo, AutowireDescriptor> LoadFrom(
            TypeInfo baseType,
            IServiceProvider serviceProvider,
            IEnumerable<TypeInfo> removes = null,
            DependencyContext dependencyContext = null)
        {
            var instances = serviceProvider.GetServices(baseType);
            Dictionary<TypeInfo, AutowireDescriptor> descriptors = new Dictionary<TypeInfo, AutowireDescriptor>();
            var kernelService = serviceProvider.GetService<IKernelService>();
            foreach (var item in instances)
            {
                var serviceType = item.GetType().GetTypeInfo();
                if (!removes.IsNullOrEmpty() && removes.Contains(serviceType))
                    continue;
                if (descriptors.ContainsKey(serviceType))
                    continue;

                var afterOnAttribute = serviceType.GetCustomAttribute<AfterOnAttribute>();
                var afters = afterOnAttribute?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();
                var beforeOnAttribute = serviceType.GetCustomAttribute<BeforeOnAttribute>();
                var befores = beforeOnAttribute?.Types?.Where(r => r.IsSubTypeOf(baseType))?.ToArray();

                descriptors.Add(serviceType, new InnerAutowireDescriptor
                {
                    After = afters,
                    Before = befores,
                    ServiceType = serviceType,
                    Status = InitStatus.Waiting,
                    ServiceInstance = item,
                    AfterCancelOn = afterOnAttribute?.CancelOn
                });
            }

            // 根据AfterOn依赖规则,去掉不需要装载的类型
            List<TypeInfo> nomatchs = new List<TypeInfo>();
            foreach (var item in descriptors)
            {
                if (!item.Value.After.IsNullOrEmpty())
                {
                    if (item.Value.AfterCancelOn == CancelOn.AllNotExists
                        && !item.Value.After.All(r => descriptors.ContainsKey(r)))
                    {
                        nomatchs.Add(item.Key);
                    }
                    else if (item.Value.AfterCancelOn == CancelOn.AnyNotExists
                        && item.Value.After.Any(r => !descriptors.ContainsKey(r)))
                    {
                        nomatchs.Add(item.Key);
                    }
                }
            }
            foreach (var item in nomatchs)
                descriptors.Remove(item);

            // 计算DependsBefore/DependsAfter字段
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
