// ReSharper disable UnusedMemberInSuper.Global
using System;

namespace Shashlik.EfCore
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

    /// <summary>
    /// 软删除, long时间戳类型
    /// </summary>
    public interface ISoftDeletedLong : ISoftDeleted<long>
    {

    }

    /// <summary>
    /// 软删除, DateTime时间类型
    /// </summary>
    public interface ISoftDeletedDateTime : ISoftDeleted<DateTime>
    {

    }
}