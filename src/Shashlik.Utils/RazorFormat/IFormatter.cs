﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Utils.RazorFormat
{
    /// <summary>
    /// 格式化器
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// 函数名,英文字母开头,可以包含大小写字母/数字/下划线,最多16位
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 格式化方法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="expression">表达式内容,action内部的内容</param>
        /// <returns>格式化后的内容</returns>
        string Format(string value, string expression);
    }
}
