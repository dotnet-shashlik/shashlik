using System;
using System.Collections.Generic;
using Shashlik.Utils.Extensions;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable IdentifierTypo
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 自动装配
    /// </summary>
    /// <typeparam name="T">装配类型</typeparam>
    public class AutowireDescriptor<T> where T : IAutowire
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="implementationType">实现类</param>
        /// <param name="afterAt">在谁之后</param>
        /// <param name="beforeAt">在谁之前</param>
        /// <param name="order">装配顺序</param>
        public AutowireDescriptor(Type implementationType, Type afterAt, Type beforeAt, int order)
        {
            if (afterAt != null && afterAt.IsSubTypeOf<T>())
                AfterAt = afterAt;
            if (beforeAt != null && beforeAt.IsSubTypeOf<T>())
                BeforeAt = beforeAt;
            Order = order;
            ImplementationType = implementationType!;
        }

        /// <summary>
        /// 装配顺序
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 在谁之后
        /// </summary>
        public Type AfterAt { get; }

        /// <summary>
        /// 在谁之前
        /// </summary>
        public Type BeforeAt { get; }

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
        public Type ImplementationType { get; }

        /// <summary>
        /// ServiceType 实例
        /// </summary>
        public T ServiceInstance { get; set; }

        public override bool Equals(object obj)
        {
            if (ImplementationType == null)
                return false;
            return ImplementationType == (Type) obj;
        }

        public override int GetHashCode()
        {
            return ImplementationType.GetHashCode();
        }
    }
}