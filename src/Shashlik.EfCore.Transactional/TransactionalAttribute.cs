using System;
using System.Data;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Utils.Extensions;

// ReSharper disable MethodHasAsyncOverload

// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.EfCore.Transactional
{
    /// <summary>
    /// 特性事务
    /// </summary>
    public class TransactionalAttribute : AbstractInterceptorAttribute
    {
        /// <summary>
        /// 默认的特性事务上下文类型
        /// </summary>
        internal static Type? DefaultDbContextType { get; set; }

        /// <summary>
        /// 使用默认的数据库上下文作为特性事务
        /// </summary>
        public TransactionalAttribute()
        {
            DbContextType = DefaultDbContextType ?? throw new InvalidOperationException(
                $"Must define DefaultTransactionalAttribute on your DbContext Type");
        }

        /// <summary>
        /// 特性事务
        /// </summary>
        /// <param name="dbContextType">数据库上下文类型</param>
        public TransactionalAttribute(Type dbContextType)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf(typeof(DbContext)))
                throw new ArgumentException($"{nameof(dbContextType)} must be subtype of DbContext");
            DbContextType = dbContextType;
        }

        public Type DbContextType { get; }

        /// <summary>
        /// 事务隔离级别,null默认隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; } = null;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (!context.ProxyMethod.IsVirtual)
                throw new InvalidOperationException(
                    $"Transactional must used on virtual method, please check your method definition: {context.ProxyMethod}");
            var efNestedTransactionType = typeof(IEfNestedTransaction<>).MakeGenericType(DbContextType);
            var efNestedTransaction =
                context.ServiceProvider.GetRequiredService(efNestedTransactionType) as IEfNestedTransaction;

            await using var tran = efNestedTransaction!.Begin(IsolationLevel);
            try
            {
                await next(context);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}