using System;
using System.Collections.Concurrent;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;

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
            if (!_beginFunction.hasValue)
            {
                _beginFunction.function = GlobalKernelServiceProvider.KernelServiceProvider
                    .GetService<IEfNestedTransactionBeginFunction<TDbContext>>();
                _beginFunction.hasValue = true;
            }
        }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private static (bool hasValue, IEfNestedTransactionBeginFunction<TDbContext>? function) _beginFunction;

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public virtual IDbContextTransaction? Current => DbContext.Database.CurrentTransaction;

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin(IsolationLevel? isolationLevel = null)
        {
            if (Current != null)
                return Current;

            IDbContextTransaction tran;
            if (_beginFunction.function is null)
                tran = isolationLevel.HasValue
                    ? DbContext.Database.BeginTransaction(isolationLevel.Value)
                    : DbContext.Database.BeginTransaction();
            else
                tran = _beginFunction.function.BeginTransaction(DbContext, isolationLevel);

            return tran;
        }
    }
}