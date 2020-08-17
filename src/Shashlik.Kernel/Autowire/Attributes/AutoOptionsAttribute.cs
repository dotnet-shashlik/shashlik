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
            if (string.IsNullOrWhiteSpace(section))
            {
                throw new ArgumentException($"“{nameof(section)}”不能为 Null 或空白", nameof(section));
            }

            Section = section;
        }

        /// <summary>
        /// 配置节点名称，例：Logger:Microsft
        /// </summary>
        public string Section { get; set; }
    }
}
