﻿using System;

namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 在指定类型之前进行装配, 装配类型有效
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BeforeAtAttribute : Attribute
    {
        public BeforeAtAttribute(Type type)
        {
            BeforeAt = type;
        }

        public Type BeforeAt { get; set; }
    }
}