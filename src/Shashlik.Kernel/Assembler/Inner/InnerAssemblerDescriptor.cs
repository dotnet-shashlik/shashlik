using System;

namespace Shashlik.Kernel.Assembler.Inner
{
    /// <summary>
    /// 内部描述器,执行状态不应该暴露出去
    /// </summary>
    internal class InnerAssemblerDescriptor<T> : AssemblerDescriptor<T> where T : IAssembler
    {
        public AutowireStatus Status { get; set; }

        public InnerAssemblerDescriptor(Type implementationType, Type? afterAt, Type? beforeAt, int order, T serviceInstance) : base(
            implementationType, afterAt, beforeAt, order, serviceInstance)
        {
        }
    }
}