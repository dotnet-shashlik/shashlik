using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Exceptions;

namespace Shashlik.Kernel.Dependency
{
    public class ShashlikServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// 注册条件集合
        /// </summary>
        public List<ConditionDescriptor> Conditions { get; }

        public ShashlikServiceDescriptor(ServiceDescriptor originalServiceDescriptor,
            List<ConditionDescriptor> conditions) : base(originalServiceDescriptor.ServiceType,
            originalServiceDescriptor.ImplementationType!, originalServiceDescriptor.Lifetime)
        {
            if (ImplementationType == null)
                throw new KernelServiceException("ShashlikServiceDescriptor implementation type can't be null");

            Conditions = conditions;
        }
    }
}