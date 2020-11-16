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

        public object? Format(object? value, string expression)
        {
            return value is null ? value : $"{expression}{value}";
        }
    }
}