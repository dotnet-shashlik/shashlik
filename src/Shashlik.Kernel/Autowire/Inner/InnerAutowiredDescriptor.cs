using System;
using System.Reflection;
using Shashlik.Kernel.Autowire;

namespace Shashlik.Kernel.Autowired.Inner
{
    /// <summary>
    /// 内部描述器,执行状态不应该暴露出去
    /// </summary>
    internal class InnerAutowiredDescriptor<T> : AutowireDescriptor<T> where T : IAutowire
    {
        public AutowireStatus Status { get; set; }

        public InnerAutowiredDescriptor(Type implementationType, Type afterAt, Type beforeAt, int order) : base(
            implementationType, afterAt, beforeAt, order)
        {
        }
    }
}