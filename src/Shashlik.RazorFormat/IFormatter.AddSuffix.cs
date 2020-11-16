#nullable enable
using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /**
     * 
     * addSuffix格式化器
     * e.g.  value = "abc"
     * addSuffix(...) -> "abc..."
     */
    /// <summary>
    /// switch格式化器
    /// </summary>
    public class AddSuffixFormatter : IFormatter
    {
        public string Action => "addSuffix";

        public string? Format(string? value, string expression)
        {
            return $"{expression}{value}";
        }
    }
}