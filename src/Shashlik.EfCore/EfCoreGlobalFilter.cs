using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shashlik.EfCore
{
    /// <summary>
    /// EfCore全局过滤器
    /// </summary>
    public class EfCoreGlobalFilter
    {
        static ConcurrentDictionary<Type, LambdaExpression> _filters = new ConcurrentDictionary<Type, LambdaExpression>();

        /// <summary>
        /// 增加EfCore全局过滤器
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="expression"></param>
        public static bool TryAddGlobalFilter<TFilter>(Expression<Func<TFilter, bool>> expression)
        {
            return _filters.TryAdd(typeof(TFilter), expression);
        }

        /// <summary>
        /// 如果需要注册,就注册实体的全局过滤器
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="builder"></param>
        public static void RegisterQueryFilterIfRequire(Type entityType, EntityTypeBuilder builder)
        {
            foreach (var filter in _filters)
            {
                if (filter.Key.IsAssignableFrom(entityType))
                {
                    builder.HasQueryFilter(filter.Value);
                }
            }
        }
    }
}
