// ReSharper disable UnusedMemberInSuper.Global
using System;
using System.Linq.Expressions;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 软删除标记
    /// </summary>
    public interface ISoftDeleted
    {
        static ISoftDeleted()
        {
            EfCoreGlobalFilter.TryAddGlobalFilter<ISoftDeleted>(r => !r.IsDeleted);
        }

        /// <summary>
        ///  是否已删除
        /// </summary>
        bool IsDeleted { get; set; }
    }

    /// <summary>
    /// 软删除标记,泛型为删除时间的类型
    /// </summary>
    public interface ISoftDeleted<T> : ISoftDeleted
        where T : struct
    {
        /// <summary>
        /// 删除时间
        /// </summary>
        T? DeleteTime { get; set; }
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