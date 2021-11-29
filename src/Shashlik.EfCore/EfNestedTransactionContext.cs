using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using Shashlik.Utils.Extensions;
using IsolationLevel = System.Data.IsolationLevel;

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
        /// 使用默认方式开启嵌套事务(异步)
        /// </summary>
        /// <param name="dbContext">DbContext上下文</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransactionByAsync<TDbContext>(
            this TDbContext dbContext,
            IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return Begin(typeof(TDbContext), dbContext, true, isolationLevel);
        }

        /// <summary>
        /// 自定义事务异步启动方式开启嵌套事务
        /// </summary>
        /// <param name="dbContext">DbContext</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction<TDbContext>(this TDbContext dbContext, IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return Begin(typeof(TDbContext), dbContext, false, isolationLevel);
        }

        /// <summary>
        /// 自定义事务启动方式开启嵌套事务
        /// </summary>
        /// <param name="dbContextType">上下文类型</param>
        /// <param name="serviceProvider">获取上下文的IServiceProvider</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction(Type dbContextType, IServiceProvider serviceProvider, IsolationLevel?
            isolationLevel = null)
        {
            return Begin(dbContextType, (DbContext)serviceProvider.GetRequiredService(dbContextType), false, isolationLevel);
        }

        #region private method

        private static IDbContextTransaction Begin(Type dbContextType, DbContext dbContext, bool useAsync, IsolationLevel? isolationLevel)
        {
            var asyncLocal = TransactionContext.GetOrAdd(dbContextType, _ => new AsyncLocal<ShashlikDbContextTransaction>());
            if (asyncLocal.Value is not null && asyncLocal.Value.IsDispose)
            {
                var newAsyncLocal = new AsyncLocal<ShashlikDbContextTransaction>();
                if (TransactionContext.TryUpdate(dbContextType, newAsyncLocal, asyncLocal))
                    asyncLocal = newAsyncLocal;
                else
                    throw new InvalidOperationException();
            }

            if (asyncLocal.Value is not null && asyncLocal.Value.IsDone)
                throw new InvalidOperationException("Already committed or rolled back.");
            if (asyncLocal.Value is null)
            {
                var originalTransaction = useAsync
                    ? BeginByAsync(dbContextType, dbContext, isolationLevel)
                    : BeginBySync(dbContextType, dbContext, isolationLevel);
                asyncLocal.Value = new ShashlikDbContextTransaction(originalTransaction, true);
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

        private static IDbContextTransaction BeginByAsync(Type dbContextType, DbContext dbContext, IsolationLevel? isolationLevel)
        {
            var beginFunction = GetEfNestedTransactionBeginFunction(dbContextType);

            IDbContextTransaction originalTransaction;
            if (beginFunction is null)
                originalTransaction = isolationLevel.HasValue
                    ? dbContext.Database.BeginTransactionAsync(isolationLevel.Value).ConfigureAwait(false).GetAwaiter().GetResult()
                    : dbContext.Database.BeginTransactionAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            else
                originalTransaction = beginFunction.BeginTransactionAsync(dbContext).ConfigureAwait(false).GetAwaiter().GetResult();

            return originalTransaction;
        }

        private static IEfNestedTransactionBeginFunction? GetEfNestedTransactionBeginFunction(Type dbContextType)
        {
            var beginFunctionDefinitionType = typeof(IEfNestedTransactionBeginFunction<>).MakeGenericType(dbContextType);
            if (GlobalKernelServiceProvider.KernelServiceProvider is null)
                throw new InvalidOperationException("GlobalKernelServiceProvider.KernelServiceProvider is null, make sure invoke UseShashlik().");
            var beginFunction =
                (IEfNestedTransactionBeginFunction?)GlobalKernelServiceProvider.KernelServiceProvider.GetService(beginFunctionDefinitionType);

            return beginFunction;
        }

        #endregion
    }
}