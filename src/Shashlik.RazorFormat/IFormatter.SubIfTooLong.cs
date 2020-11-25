using System;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /**
     *    subIfTooLong格式化器,
     *    e.g.  value = "1234567890"
     *    subIfTooLong(3,...) -> result: "123..." 
     */
    /// <summary>
    /// subIfTooLong格式化器
    /// </summary>
    public class SubIfTooLongFormatter : IFormatter
    {
        public string Action => "subIfTooLong";

        public object? Format(object? value, string expression)
        {
            if (value is null)
                return value;
            var args = expression.Split(',');
            if (args.Length == 0 || args.Length > 2)
                throw new FormatException($"{Action}({expression})");
            if (!args[0].Trim().TryParse<int>(out var length) || length < 0)
                throw new FormatException($"{Action}({expression})");
            string suffix = "...";
            if (args.Length == 2)
                suffix = args[1];

            return value.ToString().SubStringIfTooLong(length, suffix);
        }
    }
}