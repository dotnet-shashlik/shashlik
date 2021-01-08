using System;
using System.Collections.Concurrent;
using System.Data;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;

namespace Shashlik.EfCore
{
    public static class EfNestedTransactionContext
    {
        private static readonly ConcurrentDictionary<Type, AsyncLocal<ShashlikDbContextTransaction>> TransactionContext =
            new ConcurrentDictionary<Type, AsyncLocal<ShashlikDbContextTransaction>>();

        /// <summary>
        /// 获取当前上下文嵌套事务对象
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction? GetCurrent<TDbContext>()
        {
            return TransactionContext.GetOrDefault(typeof(TDbContext))?.Value;
        }

        /// <summary>
        /// 获取当前上下文嵌套事务对象
        /// </summary>
        /// <param name="_"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction? GetCurrentNestedTransaction<TDbContext>(this TDbContext _) where TDbContext : DbContext
        {
            return TransactionContext.GetOrDefault(typeof(TDbContext))?.Value;
        }

        /// <summary>
        /// 使用默认方式开启嵌套事务(同步)
        /// </summary>
        /// <param name="dbContext">DbContext上下文</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction<TDbContext>(
            this TDbContext dbContext,
            IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return BeginNestedTransaction(dbContext,
                r => isolationLevel.HasValue
                    ? dbContext.Database.BeginTransaction(isolationLevel.Value)
                    : dbContext.Database.BeginTransaction());
        }

        /// <summary>
        /// 使用默认方式开启嵌套事务(异步)
        /// </summary>
        /// <param name="dbContext">DbContext上下文</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransactionWithAsync<TDbContext>(
            this TDbContext dbContext,
            IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return BeginNestedTransaction(dbContext,
                r => isolationLevel.HasValue
                    ? dbContext.Database.BeginTransactionAsync(isolationLevel.Value).GetAwaiter().GetResult()
                    : dbContext.Database.BeginTransactionAsync().GetAwaiter().GetResult());
        }

        /// <summary>
        /// 根据自定义的IEfNestedTransactionBeginFunction(自动从全局服务上下文获取)开启嵌套事务
        /// </summary>
        /// <param name="dbContext">DbContext上下文</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransactionWithGeneral<TDbContext>(this TDbContext dbContext)
            where TDbContext : DbContext
        {
            var beginFunction = GlobalKernelServiceProvider.KernelServiceProvider!
                .GetRequiredService<IEfNestedTransactionBeginFunction<TDbContext>>();
            return BeginNestedTransaction(dbContext,
                r => beginFunction.BeginTransactionAsync(dbContext).GetAwaiter().GetResult());
        }

        /// <summary>
        /// 自定义事务异步启动方式开启嵌套事务
        /// </summary>
        /// <param name="dbContext">DbContext</param>
        /// <param name="beginFunction">自定义开始事务的方式</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction<TDbContext>(this TDbContext dbContext,
            Func<TDbContext, IDbContextTransaction> beginFunction)
            where TDbContext : DbContext
        {
            var asyncLocal = TransactionContext.GetOrAdd(typeof(TDbContext), _ => new AsyncLocal<ShashlikDbContextTransaction>());
            if (asyncLocal.Value != null && asyncLocal.Value.IsDispose)
            {
                var newAsyncLocal = new AsyncLocal<ShashlikDbContextTransaction>();
                if (TransactionContext.TryUpdate(typeof(TDbContext), newAsyncLocal, asyncLocal))
                    asyncLocal = newAsyncLocal;
                else
                    throw new InvalidOperationException();
            }

            if (asyncLocal.Value != null && asyncLocal.Value.IsDone)
                throw new InvalidOperationException("Already committed or rolled back.");
            if (asyncLocal.Value is null)
            {
                IDbContextTransaction originalTransaction = beginFunction(dbContext);
                asyncLocal.Value = new ShashlikDbContextTransaction(originalTransaction, true);
                return asyncLocal.Value;
            }

            return new ShashlikDbContextTransaction(asyncLocal.Value, false);
        }

        /// <summary>
        /// 自定义事务启动方式开启嵌套事务
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="beginFunction">自定义开始事务的方式(异步开启)</param>
        /// <param name="dbContextType"></param>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction(
            Type dbContextType,
            IServiceProvider serviceProvider,
            Func<DbContext, IDbContextTransaction> beginFunction)
        {
            var asyncLocal = TransactionContext.GetOrAdd(dbContextType, _ => new AsyncLocal<ShashlikDbContextTransaction>());
            if (asyncLocal.Value != null && asyncLocal.Value.IsDispose)
            {
                var newAsyncLocal = new AsyncLocal<ShashlikDbContextTransaction>();
                if (TransactionContext.TryUpdate(dbContextType, newAsyncLocal, asyncLocal))
                    asyncLocal = newAsyncLocal;
                else
                    throw new InvalidOperationException();
            }

            if (asyncLocal.Value != null && asyncLocal.Value.IsDone)
                throw new InvalidOperationException("Already committed or rolled back.");

            if (asyncLocal.Value is null)
            {
                var dbContext = (DbContext) serviceProvider.GetRequiredService(dbContextType);
                IDbContextTransaction originalTransaction = beginFunction(dbContext);
                asyncLocal.Value = new ShashlikDbContextTransaction(originalTransaction, true);
                return asyncLocal.Value;
            }

            return new ShashlikDbContextTransaction(asyncLocal.Value, false);
        }
    }
}