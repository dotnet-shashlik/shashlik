using System;

namespace Shashlik.Kernel.Automatic.Attributes
{
    /// <summary>
    /// 自动装配options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoOptionsAttribute : Attribute
    {
        /// <summary>
        /// 配置节点名称，例：Logger:Microsft
        /// </summary>
        public string Section { get; set; }
    }
}
