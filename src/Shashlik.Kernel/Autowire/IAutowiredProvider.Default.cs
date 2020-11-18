using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Autowire.Inner;
using Shashlik.Utils.Extensions;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

// ReSharper disable UseDeconstruction
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Shashlik.Kernel.Autowire
{
    public class DefaultAutowireProvider<T> : IAutowireProvider<T> where T : IAutowire
    {
        private void Invoke(InnerAutowiredDescriptor<T>? descriptor,
            IDictionary<Type, AutowireDescriptor<T>> autoServices,
            Action<AutowireDescriptor<T>> initAction)
        {
            if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

            switch (descriptor.Status)
            {
                case AutowireStatus.Done:
                    return;
                // 递归中发现挂起的服务那就是有循环依赖
                case AutowireStatus.Hangup:
                    throw new Exception($"exists circular dependencies on {descriptor.ImplementationType}.");
            }

            // 在这个类型之前存在依赖项
            if (!descriptor.Prevs.IsNullOrEmpty())
            {
                descriptor.Status = AutowireStatus.Hangup;
                foreach (var item in descriptor.Prevs)
                    Invoke(autoServices[item] as InnerAutowiredDescriptor<T>, autoServices, initAction);
            }

            initAction(descriptor);
            descriptor.Status = AutowireStatus.Done;
        }

        public void Autowire(IDictionary<Type, AutowireDescriptor<T>> autowiredService,
            Action<AutowireDescriptor<T>> autowiredAction)
        {
            foreach (var autowiredServiceValue in autowiredService.Values.OrderBy(r => r.Order))
            {
                Invoke(autowiredServiceValue as InnerAutowiredDescriptor<T>, autowiredService, autowiredAction);
            }
        }

        public IDictionary<Type, AutowireDescriptor<T>> Load(IKernelServices kernelServices,
            IServiceProvider serviceProvider)
        {
            var type = typeof(T);

            var instances = serviceProvider.GetServices<T>();
            // var serviceDescriptors
            //     = kernelServices.Services.Where(r => r.ServiceType == type).ToList();

            var dic = new Dictionary<Type, AutowireDescriptor<T>>();
            foreach (var instance in instances)
            {
                var implementationType = instance.GetType();
                var afterAtAttribute = implementationType.GetCustomAttribute<AfterAtAttribute>(false);
                var beforeAtAttribute = implementationType.GetCustomAttribute<BeforeAtAttribute>(false);
                var orderAttribute = implementationType.GetCustomAttribute<OrderAttribute>(false);

                var descriptor = new InnerAutowiredDescriptor<T>(
                    implementationType,
                    afterAtAttribute?.AfterAt,
                    beforeAtAttribute?.BeforeAt,
                    orderAttribute?.Order ?? 0,
                    instance);

                dic.Add(implementationType, descriptor);
            }

            foreach (var item in dic)
            {
                if (item.Value.AfterAt != null
                    && item.Value.AfterAt.IsSubTypeOf(type)
                    && dic.ContainsKey(item.Value.AfterAt))
                {
                    item.Value.Prevs.Add(item.Value.AfterAt);
                    dic[item.Value.AfterAt].Nexts.Add(item.Key);
                }

                if (item.Value.BeforeAt != null
                    && item.Value.BeforeAt.IsSubTypeOf(type)
                    && dic.ContainsKey(item.Value.BeforeAt))
                {
                    item.Value.Nexts.Add(item.Value.BeforeAt);
                    dic[item.Value.BeforeAt].Prevs.Add(item.Key);
                }
            }

            return dic;
        }
    }
}