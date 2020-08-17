using Shashlik.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shashlik.Utils.Paging
{
    public static class OrderByExtensions
    {
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="field"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, OrderByBase<TEntity> order, string field, string orderType)
        {
            field = field?.Trim();
            if (field.IsNullOrWhiteSpace())
                return order.Get(null).Func(query);

            orderType = orderType?.Trim();
            if (orderType.IsNullOrWhiteSpace() || (!orderType.EqualsIgnoreCase("asc") && !orderType.EqualsIgnoreCase("desc")))
                orderType = "asc";

            return order.Get($"{field}.{orderType}").Func(query);
        }

        /// <summary>
        /// 排序并分页
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TPageInput"></typeparam>
        /// <param name="query"></param>
        /// <param name="order"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Paging<TEntity, TPageInput>(this IQueryable<TEntity> query, OrderByBase<TEntity> order, TPageInput input)
            where TPageInput : PageInput
        {
            return
                query
                     .OrderBy(order, input.OrderField, input.OrderType)
                     .Paging(input.PageIndex, input.PageSize);
        }
    }
}
