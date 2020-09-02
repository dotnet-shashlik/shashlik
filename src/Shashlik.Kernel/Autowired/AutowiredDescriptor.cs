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
        public AutowiredDescriptor(TypeInfo? afterAt, TypeInfo? beforeAt, TypeInfo serviceType)
        {
            AfterAt = afterAt;
            BeforeAt = beforeAt;
            ServiceType = serviceType;
        }

        /// <summary>
        /// 在谁之后
        /// </summary>
        public TypeInfo? AfterAt { get; set; }

        /// <summary>
        /// 在谁之前
        /// </summary>
        public TypeInfo? BeforeAt { get; set; }

        /// <summary>
        /// 在我之前有哪些依赖
        /// </summary>
        public List<TypeInfo> Prevs { get; } = new List<TypeInfo>();

        /// <summary>
        /// 在我之后有哪些依赖
        /// </summary>
        public List<TypeInfo> Nexts { get; } = new List<TypeInfo>();
        
        /// <summary>
        /// 自动装配服务类执行
        /// </summary>
        public TypeInfo ServiceType { get; set; }

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
            return ServiceType.Equals(((AutowiredDescriptor) obj)?.ServiceType);
        }

        public override int GetHashCode()
        {
            return ServiceType.GetHashCode();
        }
    }
}