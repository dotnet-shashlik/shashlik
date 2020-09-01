using System.Reflection;

namespace Shashlik.Kernel.Autowired.Inner
{
    /// <summary>
    /// 内部描述器,执行状态不应该暴露出去
    /// </summary>
    class InnerAutowiredDescriptor : AutowiredDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="afterAt"></param>
        /// <param name="beforeAt"></param>
        /// <param name="serviceType"></param>
        /// <param name="status"></param>
        public InnerAutowiredDescriptor(TypeInfo afterAt, TypeInfo beforeAt, TypeInfo serviceType, InitStatus status)
            : base(afterAt, beforeAt, serviceType)
        {
            Status = status;
        }

        public InitStatus Status { get; set; }
    }
}