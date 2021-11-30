using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Exceptions;
using System;

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

        public override bool Equals(object? obj)
        {
            return obj is ShashlikServiceDescriptor descriptor &&
                   EqualityComparer<Type>.Default.Equals(ServiceType, descriptor.ServiceType) &&
                   EqualityComparer<Type?>.Default.Equals(ImplementationType, descriptor.ImplementationType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ServiceType, ImplementationType);
        }
    }
}