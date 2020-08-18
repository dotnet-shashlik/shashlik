using System;
using System.Linq;
using System.Reflection;

namespace Shashlik.Kernel.Autowire.Attributes
{
    /// <summary>
    /// 在指定类型之前进行装配,会明确依赖,如果这些类型有一个不
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AfterOnAttribute : Attribute
    {
        public AfterOnAttribute(params Type[] types)
        {
            Types = types.Select(r => r.GetTypeInfo()).ToArray();
        }

        /// <summary>
        /// 依赖类型
        /// </summary>
        public TypeInfo[] Types { get; set; }

        /// <summary>
        /// 装配取消条件
        /// </summary>
        public CancelOn CancelOn { get; set; } = CancelOn.AnyNotExists;
    }

    /// <summary>
    /// 装配取消条件
    /// </summary>
    public enum CancelOn
    {
        /// <summary>
        /// 任何一项不存在
        /// </summary>
        AnyNotExists,
        /// <summary>
        /// 所有项都不存在
        /// </summary>
        AllNotExists,
        /// <summary>
        /// 不取消
        /// </summary>
        Never
    }
}
