using Shashlik.Kernel.Autowire.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    public class AutowireDescriptor
    {
        /// <summary>
        /// 在我之后有哪些依赖
        /// </summary>
        public List<TypeInfo> DependsAfter { get; } = new List<TypeInfo>();
        /// <summary>
        /// 在我之前有哪些依赖
        /// </summary>
        public List<TypeInfo> DependsBefore { get; } = new List<TypeInfo>();

        /// <summary>
        /// 在哪些依赖之后
        /// </summary>
        public TypeInfo[] After { get; set; }

        /// <summary>
        /// 装配取消的条件
        /// </summary>
        public CancelOn? AfterCancelOn { get; set; }

        /// <summary>
        /// 在哪些依赖之前
        /// </summary>
        public TypeInfo[] Before { get; set; }

        /// <summary>
        /// 自动装配服务类执行
        /// </summary>
        public TypeInfo ServiceType { get; set; }

        /// <summary>
        /// ServiceType 实例
        /// </summary>
        public object ServiceInstance { get; set; }
    }
}
