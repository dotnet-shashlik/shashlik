// ReSharper disable UnusedMemberInSuper.Global

namespace Shashlik.EfCore.Filter
{
    /// <summary>
    /// 软删除标记
    /// </summary>
    public interface ISoftDeleted
    {
        /// <summary>
        ///  是否已删除
        /// </summary>
        bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 软删除标记,泛型为删除时间的类型
    /// </summary>
    public interface ISoftDeleted<TTime> : ISoftDeleted
        where TTime : struct
    {
        /// <summary>
        /// 删除时间
        /// </summary>
        TTime? DeleteTime { get; set; }
    }
}