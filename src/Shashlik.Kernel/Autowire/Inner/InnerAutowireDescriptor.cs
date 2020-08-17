using System.Collections.Generic;
using System.Reflection;

namespace Shashlik.Kernel.Autowire
{
    /// <summary>
    /// 内部描述器,执行状态不应该暴露出去
    /// </summary>
    class InnerAutowireDescriptor : AutowireDescriptor
    {
        public InitStatus Status { get; set; }
    }
}
