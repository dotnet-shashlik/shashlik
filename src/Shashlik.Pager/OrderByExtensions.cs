using System.Linq;
using Shashlik.Utils.Extensions;

namespace Shashlik.Pager
{
    public static class OrderByExtensions
    {
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="order"></param>
        /// <param name="field"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query,
            OrderByBase<TEntity> order, string field, OrderType orderType)
        {
            field = field?.Trim();
            if (field.IsNullOrWhiteSpace())
                return order.Get(null).OrderByFunction(query);

            return order.Get($"{field}.{orderType}").OrderByFunction(query);
        }

        /// <summary>
        /// 排序并分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="order"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> DoPage<TEntity>(this IQueryable<TEntity> query,
            OrderByBase<TEntity> order, PageInput input)
        {
            return
                query
                    .OrderBy(order, input.OrderField, input.OrderType)
                    .DoPage(input.PageIndex, input.PageSize);
        }

        /// <summary>
        /// 排序并分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> DoPage<TEntity>(this IQueryable<TEntity> query,
            PageInput input)
        {
            return query.DoPage(input.PageIndex, input.PageSize);
        }
    }
}