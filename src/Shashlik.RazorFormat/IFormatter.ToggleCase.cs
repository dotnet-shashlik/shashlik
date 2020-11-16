#nullable enable
using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /**
     * 
     * toggleCase格式化器
     * e.g.  value = "abc"
     * toggleCase(upper) -> "ABC"
     * toggleCase(firstUpper) -> "Abc"
     * e.g.  value = "ABC"
     * toggleCase(lower) -> "abc"
     * toggleCase(firstLower) -> "aBC"
     */
    /// <summary>
    /// switch格式化器
    /// </summary>
    public class ToggleCaseFormatter : IFormatter
    {
        public string Action => "toggleCase";

        public string? Format(string? value, string expression)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return expression switch
            {
                "upper" => value.ToUpperInvariant(),
                "lower" => value.ToLowerInvariant(),
                "firstUpper" => value.UpperFirstCase(),
                "firstLower" => value.LowerFirstCase(),
                _ => throw new FormatException($"{Action}({expression})")
            };
        }
    }
}