// ReSharper disable InconsistentNaming
namespace Shashlik.Kernel.Attributes
{
    /// <summary>
    /// 条件类型
    /// </summary>
    public enum ConditionType : byte
    {
        /// <summary>
        /// 所有都满足
        /// </summary>
        ALL,
        /// <summary>
        /// 满足任意一个
        /// </summary>
        ANY
    }
}
