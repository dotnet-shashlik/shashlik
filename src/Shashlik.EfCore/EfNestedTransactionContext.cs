using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;
using IsolationLevel = System.Data.IsolationLevel;

namespace Shashlik.EfCore
{
    public static class EfNestedTransactionContext
    {
        private static readonly ConcurrentDictionary<DbContext, AsyncLocal<ShashlikDbContextTransaction>> TransactionContext = new();

        /// <summary>
        /// 获取当前上下文嵌套事务对象
        /// </summary>
        /// <param name="dbContext"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction? GetCurrentNestedTransaction<TDbContext>(this TDbContext dbContext) where TDbContext : DbContext
        {
            return TransactionContext.GetOrDefault(dbContext)?.Value;
        }

        /// <summary>
        /// 开启嵌套事务(同步)
        /// </summary>
        /// <param name="dbContext">DbContext</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction<TDbContext>(this TDbContext dbContext, IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return Begin(typeof(TDbContext), dbContext, isolationLevel);
        }

        #region private method

        private static IDbContextTransaction Begin(Type dbContextType, DbContext dbContext, IsolationLevel? isolationLevel)
        {
            var asyncLocal = TransactionContext.GetOrAdd(dbContext, _ => new AsyncLocal<ShashlikDbContextTransaction>());
            if (asyncLocal.Value is not null && asyncLocal.Value.IsDispose)
            {
                var newAsyncLocal = new AsyncLocal<ShashlikDbContextTransaction>();
                if (TransactionContext.TryUpdate(dbContext, newAsyncLocal, asyncLocal))
                    asyncLocal = newAsyncLocal;
                else
                    throw new InvalidOperationException();
            }

            if (asyncLocal.Value is not null && asyncLocal.Value.IsDone)
                throw new InvalidOperationException("Already committed or rolled back.");
            if (asyncLocal.Value is null)
            {
                var originalTransaction = BeginBySync(dbContextType, dbContext, isolationLevel);
                asyncLocal.Value =
                    new ShashlikDbContextTransaction(originalTransaction, true, () => { TransactionContext.TryRemove(dbContext, out _); });
                return asyncLocal.Value;
            }

            return new ShashlikDbContextTransaction(asyncLocal.Value, false);
        }

        private static IDbContextTransaction BeginBySync(Type dbContextType, DbContext dbContext, IsolationLevel? isolationLevel)
        {
            var beginFunction = GetEfNestedTransactionBeginFunction(dbContextType);

            IDbContextTransaction originalTransaction;
            if (beginFunction is null)
                originalTransaction = isolationLevel.HasValue
                    ? dbContext.Database.BeginTransaction(isolationLevel.Value)
                    : dbContext.Database.BeginTransaction();
            else
                originalTransaction = beginFunction.BeginTransactionAsync(dbContext).GetAwaiter().GetResult();

            return originalTransaction;
        }

        private static IEfNestedTransactionBeginFunction? GetEfNestedTransactionBeginFunction(Type dbContextType)
        {
            var beginFunctionDefinitionType = typeof(IEfNestedTransactionBeginFunction<>).MakeGenericType(dbContextType);
            if (GlobalKernelServiceProvider.KernelServiceProvider is null)
                throw new InvalidOperationException("GlobalKernelServiceProvider.KernelServiceProvider is null, make sure invoke UseShashlik().");
            return (IEfNestedTransactionBeginFunction?)GlobalKernelServiceProvider.KernelServiceProvider.GetService(beginFunctionDefinitionType);
        }

        #endregion
    }
}