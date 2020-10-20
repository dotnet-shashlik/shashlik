namespace Shashlik.RazorFormat
{
    /// <summary>
    /// 格式化器
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// 函数名,英文字母开头,可以包含大小写字母/数字/下划线,最多16位
        /// </summary>
        string Action { get; }

        /// <summary>
        /// 格式化方法
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="expression">表达式内容,action内部的内容,例:    0:未知|1:男性|2:女性|null:不男不女|empty:空|default:未知</param>
        /// <returns>格式化后的内容</returns>
        string Format(string value, string expression);
    }
}