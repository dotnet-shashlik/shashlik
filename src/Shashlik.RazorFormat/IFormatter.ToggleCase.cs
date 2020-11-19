
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

        public object? Format(object? value, string expression)
        {
            if (value is null)
                return value;

            var str = value.ToString();
            return expression switch
            {
                "upper" => str.ToUpperInvariant(),
                "lower" => str.ToLowerInvariant(),
                "firstUpper" => str.UpperFirstCase(),
                "firstLower" => str.LowerFirstCase(),
                _ => throw new FormatException($"{Action}({expression})")
            };
        }
    }
}