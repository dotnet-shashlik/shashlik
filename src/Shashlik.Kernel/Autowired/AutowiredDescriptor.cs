#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable IdentifierTypo
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Shashlik.Kernel.Autowired
{
    public class AutowiredDescriptor
    {
        public AutowiredDescriptor(Type? afterAt, Type? beforeAt, Type serviceType)
        {
            AfterAt = afterAt;
            BeforeAt = beforeAt;
            ServiceType = serviceType;
        }

        /// <summary>
        /// 在谁之后
        /// </summary>
        public Type? AfterAt { get; set; }

        /// <summary>
        /// 在谁之前
        /// </summary>
        public Type? BeforeAt { get; set; }

        /// <summary>
        /// 在我之前有哪些依赖
        /// </summary>
        public List<Type> Prevs { get; } = new List<Type>();

        /// <summary>
        /// 在我之后有哪些依赖
        /// </summary>
        public List<Type> Nexts { get; } = new List<Type>();

        /// <summary>
        /// 自动装配服务类执行
        /// </summary>
        public Type ServiceType { get; set; }

        /// <summary>
        /// ServiceType 实例
        /// </summary>
        public object? ServiceInstance { get; set; }

        /// <summary>
        /// 特性值
        /// </summary>
        public Attribute? Attribute { get; set; }

        public override bool Equals(object obj)
        {
            if (ServiceType == null)
                return false;
            return ServiceType == (Type) obj;
        }

        public override int GetHashCode()
        {
            return ServiceType.GetHashCode();
        }
    }
}