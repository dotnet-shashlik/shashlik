using System;

namespace Shashlik.Cap
{
    /// <summary>
    /// 事件名称/组名定义
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class CapNameAttribute : Attribute
    {
        public CapNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 事件名称/事件处理组名
        /// </summary>
        public string Name { get; }
    }
}