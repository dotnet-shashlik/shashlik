using System;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Autowire.Attributes
{
    /// <summary>
    /// 在指定类型之前进行装配
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BeforeAtAttribute : Attribute
    {
        public BeforeAtAttribute(Type type)
        {
            BeforeAt = type;
        }

        public Type BeforeAt { get; set; }
    }
}
