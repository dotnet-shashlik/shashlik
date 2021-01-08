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
                $"invalid default DbContext type, make sure invoke method of \"EfCoreTransactionalExtensions.UseEfCoreTransactional\"");
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

        /// <summary>
        /// 数据库上下文类型
        /// </summary>
        public Type DbContextType { get; }

        /// <summary>
        /// 事务隔离级别,null默认隔离级别
        /// </summary>
        public IsolationLevel? IsolationLevel { get; set; } = null;

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await using var tran =
                EfNestedTransactionContext.BeginNestedTransaction(
                    DbContextType,
                    context.ServiceProvider,
                    r => IsolationLevel.HasValue
                        ? r.Database.BeginTransactionAsync(IsolationLevel.Value).GetAwaiter().GetResult()
                        : r.Database.BeginTransactionAsync().GetAwaiter().GetResult());

            try
            {
                await next(context);
                await tran.CommitAsync();
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}