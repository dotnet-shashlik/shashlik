﻿using System;
using System.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{

    /**
     * 
     * 表达式格式:
     * 例：@{Gender|switch(0:未知|1:男性|2:女性|null:不男不女|empty:空|default:未知)},
     * null：空选择器，empty：空字符串选择器,default:默认选择器，只能放到最后
     * 
     */

    /// <summary>
    /// switch格式化器
    /// </summary>
    public class SwitchFormatter : IFormatter
    {
        public string Action => "switch";

        public string Format(string value, string expression)
        {
            var formats = expression.Split('|');
            string s;
            // 择值表达式
            var selectors = formats.Select(r =>
            {
                var arr = r.Split(":", StringSplitOptions.RemoveEmptyEntries);
                if (arr.Length != 2)
                    throw new FormatException($"select value format express error: {r}");
                arr[0] = arr[0].Trim();
                arr[1] = arr[1].Trim();
                return arr;
            }).ToList();
            if (selectors.IsNullOrEmpty())
                return value;

            var selectValue = selectors.FirstOrDefault(r => r[0] == value
                                                            || (r[0] == "null" && value is null) // null选择器
                                                            || (r[0] == "empty" && value.IsNullOrWhiteSpace()));
            if (selectValue != null)
                // 找到了对应的值
                s = selectValue[1];
            else if (selectors.Last()[0] == "default")
                // 最后一个选择器是不是default
                s = selectors.Last()[1];
            else
                // 没找到对应的选择器，返回原始值
                return value;

            return s;
        }
    }
}
