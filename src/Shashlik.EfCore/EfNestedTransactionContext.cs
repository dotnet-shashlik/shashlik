using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
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
        private static readonly ConcurrentDictionary<Type, AsyncLocal<ShashlikDbContextTransaction>> TransactionContext = new();

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
        /// 开启嵌套事务(异步)
        /// </summary>
        /// <param name="dbContext">DbContext上下文</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task<IDbContextTransaction> BeginNestedTransactionAsync<TDbContext>(
            this TDbContext dbContext,
            IsolationLevel? isolationLevel = null)
            where TDbContext : DbContext
        {
            return await Begin(typeof(TDbContext), dbContext, true, isolationLevel);
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
            return Begin(typeof(TDbContext), dbContext, false, isolationLevel).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 开启嵌套事务(同步)
        /// </summary>
        /// <param name="dbContextType">上下文类型</param>
        /// <param name="serviceProvider">获取上下文的IServiceProvider</param>
        /// <param name="isolationLevel">事务隔离级别，null保持默认,注册<see cref="IEfNestedTransactionBeginFunction"/>后此参数无效</param>
        /// <returns></returns>
        public static IDbContextTransaction BeginNestedTransaction(Type dbContextType, IServiceProvider serviceProvider, IsolationLevel?
            isolationLevel = null)
        {
            return Begin(dbContextType, (DbContext)serviceProvider.GetRequiredService(dbContextType), false, isolationLevel)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        #region private method

        private static async Task<IDbContextTransaction> Begin(Type dbContextType, DbContext dbContext, bool useAsync, IsolationLevel? isolationLevel)
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
                    : Task.FromResult(BeginBySync(dbContextType, dbContext, isolationLevel));
                asyncLocal.Value = new ShashlikDbContextTransaction(await originalTransaction, true);
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

        private static async Task<IDbContextTransaction> BeginByAsync(Type dbContextType, DbContext dbContext, IsolationLevel? isolationLevel)
        {
            var beginFunction = GetEfNestedTransactionBeginFunction(dbContextType);

            IDbContextTransaction originalTransaction;
            if (beginFunction is null)
                originalTransaction = isolationLevel.HasValue
                    ? await dbContext.Database.BeginTransactionAsync(isolationLevel.Value).ConfigureAwait(false)
                    : await dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            else
                originalTransaction = await beginFunction.BeginTransactionAsync(dbContext).ConfigureAwait(false);

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