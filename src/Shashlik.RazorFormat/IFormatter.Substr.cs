using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /// <summary>
    /// substr格式化器,                  <para></para>
    /// e.g.  value = "1234567890"       <para></para>
    /// substr(3,2)-> result: "45"       <para></para>
    /// substr(3)-> result: "4567890"    <para></para>
    /// </summary>
    public class SubstrFormatter : IFormatter
    {
        public string Action => "substr";

        public object? Format(object? value, string expression)
        {
            if (value is null)
                return value;
            var args = expression.Split(',');
            if (args.Length != 1 && args.Length != 2)
                throw new FormatException(expression);

            if (!args[0].Trim().TryParse<int>(out var start) || start < 0)
                throw new FormatException(expression);

            var length = 0;
            if (args.Length == 2)
                if (!args[1].Trim().TryParse(out length) || length < 0)
                    throw new FormatException(expression);

            var str = value.ToString();
            if (str is null)
                return value;
            if (start > str.Length - 1)
                return string.Empty;
            if (start + length > str.Length || length == 0)
                return str.Substring(start);
            return str.Substring(start, length);
        }
    }
}