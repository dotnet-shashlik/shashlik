using System;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Autowire.Attributes
{
    /// <summary>
    /// 在指定服务完成之后执行
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AfterAttribute : Attribute
    {
        public AfterAttribute(params Type[] types)
        {
            Types = types.Select(r => r.GetTypeInfo()).ToArray();
        }

        public TypeInfo[] Types { get; set; }
    }
}
