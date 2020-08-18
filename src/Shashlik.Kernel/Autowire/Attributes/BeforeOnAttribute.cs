using System;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Autowire.Attributes
{
    /// <summary>
    /// 在指定类型之前进行装配
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BeforeOnAttribute : Attribute
    {
        public BeforeOnAttribute(params Type[] types)
        {
            Types = types.Select(r => r.GetTypeInfo()).ToArray();
        }

        public TypeInfo[] Types { get; set; }
    }
}
