using System;

namespace Shashlik.Kernel.Autowire.Inner
{
    /// <summary>
    /// 内部描述器,执行状态不应该暴露出去
    /// </summary>
    internal class InnerAutowiredDescriptor<T> : AutowireDescriptor<T> where T : IAutowire
    {
        public AutowireStatus Status { get; set; }

        public InnerAutowiredDescriptor(Type implementationType, Type? afterAt, Type? beforeAt, int order, T serviceInstance) : base(
            implementationType, afterAt, beforeAt, order, serviceInstance)
        {
        }
    }
}