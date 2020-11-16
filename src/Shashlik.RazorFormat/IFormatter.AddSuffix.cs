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

        public object? Format(object? value, string expression)
        {
            return value is null ? value : $"{value}{expression}";
        }
    }
}