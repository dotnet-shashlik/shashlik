#nullable enable
using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /**
     * 
     * addPrefix格式化器
     * e.g.  value = "abc"
     * addPrefix(demo:) -> "demo:abc"
     */
    /// <summary>
    /// switch格式化器
    /// </summary>
    public class AddPrefixFormatter : IFormatter
    {
        public string Action => "addPrefix";

        public string? Format(string? value, string expression)
        {
            return $"{expression}{value}";
        }
    }
}