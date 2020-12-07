using System;

// ReSharper disable RedundantAttributeUsageProperty

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 自动装配options
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AutoOptionsAttribute : Attribute
    {
        /// <summary>
        /// 自动装配options
        /// </summary>
        /// <param name="section">配置节点名称，例：Logger.Microsoft</param>
        /// <param name="supportDot">是否支持小数点作为路径连接符,默认true,如果你的配置属性本身包含小数点,请设置为false</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AutoOptionsAttribute(string section, bool supportDot = true)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentNullException(nameof(section));
            SupportDot = supportDot;
            Section = SupportDot ? section.Replace(".", ":") : section;
        }

        /// <summary>
        /// 配置节点名称，例：Logger.Microsoft
        /// </summary>
        public string Section { get; }

        /// <summary>
        /// 是否支持小数点作为路径连接符,默认true
        /// </summary>
        public bool SupportDot { get; }
    }
}