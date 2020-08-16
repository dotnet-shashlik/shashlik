using System;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Automatic.Attributes
{
    /// <summary>
    /// 自动装配配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BeforeAttribute : Attribute
    {
        public BeforeAttribute(params Type[] types)
        {
            Types = types.Select(r => r.GetTypeInfo()).ToArray();
        }

        public TypeInfo[] Types { get; set; }
    }
}
