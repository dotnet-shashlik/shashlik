
using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /// <summary>
    /// toggleCase 大小写切换格式化器        <para></para>
    /// e.g.  value = "abc"                  <para></para>
    /// toggleCase(upper) -> "ABC"           <para></para>   
    /// toggleCase(firstUpper) -> "Abc"      <para></para>
    /// e.g.value = "ABC"                    <para></para>
    /// toggleCase(lower) -> "abc"           <para></para>
    /// toggleCase(firstLower) -> "aBC"      <para></para>
    /// </summary>
    public class ToggleCaseFormatter : IFormatter
    {
        public string Action => "toggleCase";

        public object? Format(object? value, string expression)
        {
            if (value is null)
                return value;

            var str = value.ToString();
            if (str is null)
                return null;
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