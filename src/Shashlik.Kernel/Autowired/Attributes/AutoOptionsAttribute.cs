using System;

namespace Shashlik.Kernel.Autowire.Attributes
{
    /// <summary>
    /// 自动装配options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoOptionsAttribute : Attribute
    {
        public AutoOptionsAttribute(string section)
        {
            Section = section ?? throw new ArgumentNullException(nameof(section));
        }

        /// <summary>
        /// 配置节点名称，例：Logger:Microsoft
        /// </summary>
        public string Section { get; set; }
    }
}