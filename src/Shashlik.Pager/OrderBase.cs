using System;
using System.Collections.Generic;
using System.Linq;

namespace Shashlik.Pager
{
    /// <summary>
    /// 排序基类,定义某个实体允许的排序方式,第一条为默认排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class OrderByBase<TEntity>
    {
        protected OrderByBase(params OrderByModel<TEntity>[] args)
        {
            foreach (var item in args)
                Orders.Add(item.OrderBy, item);
        }

        /// <summary>
        /// 定义排序方式,第一条为默认排序
        /// </summary>
        public IDictionary<string, OrderByModel<TEntity>> Orders { get; } =
            new Dictionary<string, OrderByModel<TEntity>>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 获取排序模型
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public OrderByModel<TEntity> Get(string orderBy)
        {
            // 空使用第一个,即默认排序
            if (string.IsNullOrWhiteSpace(orderBy))
                return Orders.FirstOrDefault().Value;
            if (!Orders.ContainsKey(orderBy))
                throw new ArgumentException($"Can not find sort definition of {orderBy}");

            return Orders[orderBy];
        }
    }
}