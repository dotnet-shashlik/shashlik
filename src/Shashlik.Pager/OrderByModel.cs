using System;
using System.Linq;

namespace Shashlik.Pager
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
        /// <param name="orderBy">排序定义名称: 排序字段.排序方式, 例Name.asc 不区分大小写</param>
        /// <param name="func">排序表达式</param>
        public OrderByModel(
            string orderBy,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> func
        )
        {
            OrderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            OrderByFunction = func ?? throw new ArgumentNullException(nameof(func));
        }

        public string OrderBy { get; }

        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderByFunction { get; }
    }
}