using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Assembler.Inner;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Exceptions;
using Shashlik.Utils.Extensions;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

// ReSharper disable UseDeconstruction
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Shashlik.Kernel.Assembler
{
    public class DefaultAssemblerProvider<T> : IAssemblerProvider<T> where T : IAssembler
    {
        private void Invoke(InnerAssemblerDescriptor<T>? descriptor,
            IDictionary<Type, AssemblerDescriptor<T>> autoServices,
            Action<AssemblerDescriptor<T>> initAction)
        {
            if (descriptor is null) throw new ArgumentNullException(nameof(descriptor));

            switch (descriptor.Status)
            {
                case AssembleStatus.Done:
                    return;
                // 递归中发现挂起的服务那就是有循环依赖
                case AssembleStatus.Hangup:
                    throw new KernelAssembleException($"Exists recursive dependencies on {descriptor.ImplementationType}");
            }

            // 在这个类型之前存在依赖项
            if (!descriptor.Prevs.IsNullOrEmpty())
            {
                descriptor.Status = AssembleStatus.Hangup;
                foreach (var item in descriptor.Prevs)
                    Invoke(autoServices[item] as InnerAssemblerDescriptor<T>, autoServices, initAction);
            }

            initAction(descriptor);
            descriptor.Status = AssembleStatus.Done;
        }

        public void Execute(IDictionary<Type, AssemblerDescriptor<T>> autowiredService,
            Action<AssemblerDescriptor<T>> autowiredAction)
        {
            if (autowiredService == null) throw new ArgumentNullException(nameof(autowiredService));
            if (autowiredAction == null) throw new ArgumentNullException(nameof(autowiredAction));
            foreach (var autowiredServiceValue in autowiredService.Values.OrderBy(r => r.Order))
            {
                Invoke(autowiredServiceValue as InnerAssemblerDescriptor<T>, autowiredService, autowiredAction);
            }
        }

        public IDictionary<Type, AssemblerDescriptor<T>> Load(IKernelServices kernelServices,
            IServiceProvider serviceProvider)
        {
            if (kernelServices == null) throw new ArgumentNullException(nameof(kernelServices));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            var type = typeof(T);

            var instances = serviceProvider.GetServices<T>();

            var dic = new Dictionary<Type, AssemblerDescriptor<T>>();
            foreach (var instance in instances)
            {
                var implementationType = instance.GetType();
                var afterAtAttribute = implementationType.GetCustomAttribute<AfterAtAttribute>(false);
                var beforeAtAttribute = implementationType.GetCustomAttribute<BeforeAtAttribute>(false);
                var orderAttribute = implementationType.GetCustomAttribute<OrderAttribute>(false);

                var descriptor = new InnerAssemblerDescriptor<T>(
                    implementationType,
                    afterAtAttribute?.AfterAt,
                    beforeAtAttribute?.BeforeAt,
                    orderAttribute?.Order ?? 0,
                    instance);

                dic.Add(implementationType, descriptor);
            }

            foreach (var item in dic)
            {
                if (item.Value.AfterAt is not null
                    && item.Value.AfterAt.IsSubTypeOrEqualsOf(type)
                    && dic.ContainsKey(item.Value.AfterAt))
                {
                    item.Value.Prevs.Add(item.Value.AfterAt);
                    dic[item.Value.AfterAt].Nexts.Add(item.Key);
                }

                if (item.Value.BeforeAt is not null
                    && item.Value.BeforeAt.IsSubTypeOrEqualsOf(type)
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