﻿using System;
// ReSharper disable RedundantAttributeUsageProperty

namespace Shashlik.Kernel.Attributes
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
            // 可以使用.作为属性连接符
            Section = Section.Replace(".", ":");
        }

        /// <summary>
        /// 配置节点名称，例：Logger.Microsoft
        /// </summary>
        public string Section { get; }
    }
}