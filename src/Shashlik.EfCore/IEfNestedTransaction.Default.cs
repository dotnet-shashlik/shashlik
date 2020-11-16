using System;
using System.Collections.Concurrent;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
using IsolationLevel = System.Data.IsolationLevel;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Shashlik.EfCore
{
    /// <summary>
    /// 默认的嵌套事务
    /// </summary>
    public class DefaultEfNestedTransaction<TDbContext> : IEfNestedTransaction<TDbContext>
        where TDbContext : DbContext
    {
        public DefaultEfNestedTransaction(TDbContext dbContext)
        {
            DbContext = dbContext;
            if (!_beginFunction.isSetValue)
            {
                _beginFunction.function = GlobalKernelServiceProvider.KernelServiceProvider
                    .GetService<IEfNestedTransactionBeginFunction<TDbContext>>();
                _beginFunction.isSetValue = true;
            }
        }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private static (bool isSetValue, IEfNestedTransactionBeginFunction<TDbContext>? function) _beginFunction;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public virtual IDbContextTransaction? Current =>
            _topTransaction == null || _topTransaction.IsDone ? null : _topTransaction;

        /// <summary>
        /// 顶层事务实例
        /// </summary>
        private ShashlikDbContextTransaction? _topTransaction;

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin(IsolationLevel? isolationLevel = null)
        {
            lock (DbContext)
            {
                if (DbContext.Database.CurrentTransaction is null)
                {
                    IDbContextTransaction tran;
                    if (_beginFunction.function is null)
                        tran = isolationLevel.HasValue
                            ? DbContext.Database.BeginTransaction(isolationLevel.Value)
                            : DbContext.Database.BeginTransaction();
                    else
                        tran = _beginFunction.function.BeginTransaction(DbContext, isolationLevel);

                    _topTransaction = new ShashlikDbContextTransaction(tran);
                    return _topTransaction;
                }

                if (_topTransaction == null)
                    throw new TransactionException(
                        $"DbContext.Database.CurrentTransaction is null, and inner top transaction is null.");
                return new ShashlikDbContextTransaction(_topTransaction);
            }
        }
    }
}