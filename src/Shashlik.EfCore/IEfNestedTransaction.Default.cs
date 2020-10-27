using System;
using System.Collections.Concurrent;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Shashlik.EfCore
{
    /// <summary>
    /// 默认的嵌套事务
    /// </summary>
    public class DefaultEfNestedTransaction<TDbContext> : IEfNestedTransaction<TDbContext>
        where TDbContext : DbContext
    {
        private ConcurrentStack<ShashlikDbContextTransaction<TDbContext>> Transactions { get; } =
            new ConcurrentStack<ShashlikDbContextTransaction<TDbContext>>();

        public DefaultEfNestedTransaction(IServiceProvider serviceProvider)
        {
            DbContext = serviceProvider.GetRequiredService<TDbContext>();
            BeginFunction = serviceProvider.GetService<IEfNestedTransactionBeginFunction<TDbContext>>();
        }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private IEfNestedTransactionBeginFunction<TDbContext>? BeginFunction { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public virtual IDbContextTransaction? Current
        {
            get
            {
                if (Transactions.TryPeek(out var tran) && !tran.IsDone)
                    return tran;
                return null;
            }
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin(IsolationLevel? isolationLevel = null)
        {
            if (Transactions.TryPeek(out var existsTran)
                && !existsTran.IsDone)
                return existsTran;

            IDbContextTransaction tran;
            if (BeginFunction == null)
                tran = isolationLevel.HasValue
                    ? DbContext.Database.BeginTransaction(isolationLevel.Value)
                    : DbContext.Database.BeginTransaction();
            else
                tran = BeginFunction.BeginTransaction(DbContext, isolationLevel);

            var innerTran = new ShashlikDbContextTransaction<TDbContext>(DbContext, tran);
            Transactions.Push(innerTran);
            return innerTran;
        }
    }
}