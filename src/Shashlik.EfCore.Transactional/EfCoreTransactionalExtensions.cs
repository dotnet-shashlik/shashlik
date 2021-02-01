using System;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;

namespace Shashlik.EfCore.Transactional
{
    public static class EfCoreTransactionalExtensions
    {
        /// <summary>
        /// 使用efcore特性事务
        /// </summary>
        /// <param name="builder"></param>
        /// <typeparam name="TDefaultDbContext">默认上下文类型</typeparam>
        /// <returns></returns>
        public static IHostBuilder UseEfCoreTransactional<TDefaultDbContext>(this IHostBuilder builder)
            where TDefaultDbContext : DbContext
        {
            return builder.UseEfCoreTransactional(typeof(TDefaultDbContext));
        }

        /// <summary>
        /// 使用efcore特性事务
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="defaultDbContextType">默认上下文类型</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IHostBuilder UseEfCoreTransactional(this IHostBuilder builder, Type defaultDbContextType)
        {
            if (!defaultDbContextType.IsSubType<DbContext>())
                throw new ArgumentException("Default dbContext type must inherited from DbContext.",
                    nameof(defaultDbContextType));
            TransactionalAttribute.DefaultDbContextType = defaultDbContextType;
            return builder.UseEfCoreTransactional();
        }

        /// <summary>
        /// 使用efcore特性事务，不配置默认上下文
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHostBuilder UseEfCoreTransactional(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }
    }
}