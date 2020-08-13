using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guc.Utils.Page
{
    /// <summary>
    /// 排序模型
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class OrderByModel<TEntity>
    {
        /// <summary>
        /// 排序模型
        /// </summary>
        /// <param name="orderBy">Name.asc 不区分大小写</param>
        /// <param name="func">排序表达式</param>
        public OrderByModel(
            string orderBy,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> func
            )
        {
            OrderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public string OrderBy { get; }

        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> Func { get; }
    }

}
