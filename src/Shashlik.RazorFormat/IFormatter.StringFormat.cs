namespace Shashlik.RazorFormat
{
    /**
     * 
     * stringFormat格式化器
     * e.g.  value = 1.1
     * stringFormat(f2) -> "1.10"
     */
    /// <summary>
    /// stringFormat格式化器
    /// </summary>
    public class StringFormatFormatter : IFormatter
    {
        public string Action => "stringFormat";

        public object? Format(object? value, string expression)
        {
            return value == null ? value : string.Format($"{{0:{expression}}}", value);
        }
    }
}