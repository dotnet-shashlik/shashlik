using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Kernel.Automatic
{
    /// <summary>
    /// 自动装配配置,
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoOptionsAttribute : Attribute
    {
        /// <summary>
        /// 配置节点名称，例：Logger:Microsft
        /// </summary>
        public string SectionName { get; set; }
    }
}
