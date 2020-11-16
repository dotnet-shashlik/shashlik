#nullable enable
using System;
using System.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.RazorFormat
{
    /**
     *    subIfTooLong格式化器,
     *    e.g.  value = "1234567890"
     *    subIfTooLong(3,...)
     *    result: "123..." 
     */
    /// <summary>
    /// switch格式化器
    /// </summary>
    public class SubIfTooLongFormatter : IFormatter
    {
        public string Action => "subIfTooLong";

        public string? Format(string? value, string expression)
        {
            if (value.IsNullOrWhiteSpace())
                return string.Empty;
            var args = expression.Split(',');
            if (args.Length == 0 || args.Length > 2)
                throw new FormatException(expression);

            if (!args[0].Trim().TryParse<int>(out var length) || length < 0)
                throw new FormatException(expression);

            string suffix = "...";
            if (args.Length == 2)
                suffix = args[1];

            return value.SubStringIfTooLong(length, suffix);
        }
    }
}