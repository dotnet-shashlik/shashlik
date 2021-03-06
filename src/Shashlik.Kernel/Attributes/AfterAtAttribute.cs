﻿using System;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 在指定类型之前进行装配, 装配类型有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AfterAtAttribute : Attribute
    {
        public AfterAtAttribute(Type type)
        {
            AfterAt = type;
        }

        /// <summary>
        /// 依赖类型
        /// </summary>
        public Type AfterAt { get; }
    }
}