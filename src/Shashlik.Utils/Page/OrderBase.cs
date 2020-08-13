using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Guc.Utils.Extensions;

namespace Guc.Utils.Page
{
    /// <summary>
    /// 排序基类,定义某个实体允许的排序方式,第一条为默认排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class OrderByBase<TEntity>
    {
        /// <summary>
        /// 定义排序方式,第一条为默认排序
        /// </summary>
        public abstract List<OrderByModel<TEntity>> Orders { get; }

        /// <summary>
        /// 获取排序模型
        /// </summary>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public OrderByModel<TEntity> Get(string orderBy)
        {
            if (orderBy.IsNullOrWhiteSpace())
                return Orders.FirstOrDefault();
            var order = Orders.FirstOrDefault(r => r.OrderBy.EqualsIgnoreCase(orderBy));
            if (order != null)
                return order;
            // 没找到使用默认排序
            order = Orders.FirstOrDefault();
            if (order != null)
                return order;
            // 一个都没得 则异常
            throw new Exception($"{this.GetType()}未定义排序方式");
        }
    }
}
