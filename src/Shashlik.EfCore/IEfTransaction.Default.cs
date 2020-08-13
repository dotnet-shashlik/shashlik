using Microsoft.EntityFrameworkCore.Storage;
using Guc.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;

namespace Guc.EfCore
{
    /// <summary>
    /// 默认的ef core事务
    /// </summary>
    class DefaultEfTransaction : IEfTransaction
    {
        /// <summary>
        /// 事务模型
        /// </summary>
        class TranModel
        {
            public DbContext DbContext { get; set; }

            public IDbContextTransaction TopEfTransaction { get; set; }

            public EfDbContextTransaction ScopedTransaction { get; set; }
        }

        IServiceProvider serviceProvider { get; }

        /// <summary>
        /// 所有的事务数据
        /// </summary>
        List<TranModel> Trans { get; } = new List<TranModel>();

        public DefaultEfTransaction(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IDbContextTransaction GetCurrent(DbContext dbContext)
        {
            return Trans.LastOrDefault(r => r.DbContext == dbContext && r.ScopedTransaction.IsTopTransaction)?.ScopedTransaction;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="capPublisher"></param>
        /// <returns></returns>
        public IDbContextTransaction Begin(DbContext dbContext, Func<DbContext, IDbContextTransaction> beginTransactionFunc)
        {
            if (Trans.Any(r => r.DbContext == dbContext && r.ScopedTransaction.IsTopTransaction && !r.ScopedTransaction.IsCompleted))
            {
                var last = Trans.Where(r => r.DbContext == dbContext).LastOrDefault();
                // 存在该类型的事务
                var tranModel = new TranModel
                {
                    DbContext = dbContext,
                    TopEfTransaction = last.TopEfTransaction,
                    ScopedTransaction = new EfDbContextTransaction(last.TopEfTransaction, false)
                };
                Trans.Add(tranModel);
                return tranModel.ScopedTransaction;
            }
            else
            {
                IDbContextTransaction tran;
                if (beginTransactionFunc == null)
                    tran = dbContext.Database.BeginTransaction();
                else
                    tran = beginTransactionFunc(dbContext);

                var top = new EfDbContextTransaction(tran, true);
                // 不存在该类型的事务
                var tranModel = new TranModel
                {
                    DbContext = dbContext,
                    TopEfTransaction = top,
                    ScopedTransaction = top
                };

                Trans.Add(tranModel);
                return tranModel.ScopedTransaction;
            }
        }

        /// <summary>
        /// scoped销毁时 dispose所有的事务
        /// </summary>
        public void Dispose()
        {
            foreach (var item in Trans)
                item?.ScopedTransaction?.Dispose();
        }

        public IDbContextTransaction Begin(DbContext dbContext)
        {
            return Begin(dbContext, null);
        }
    }

    class EfDbContextTransaction : IDbContextTransaction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topDbContextTransaction">顶层事务</param>
        /// <param name="isTopTransaction">是否为顶层事务</param>
        /// <param name="onTopTransactionComplete">顶层事务完成时</param>
        public EfDbContextTransaction(
            IDbContextTransaction topDbContextTransaction,
            bool isTopTransaction)
        {
            TopDbContextTransaction = topDbContextTransaction;
            TransactionId = topDbContextTransaction.TransactionId;
            IsTopTransaction = isTopTransaction;
        }


        /// <summary>
        /// 数据库事务对象
        /// </summary>
        private IDbContextTransaction TopDbContextTransaction { get; }

        /// <summary>
        /// 当前事务id
        /// </summary>
        public Guid TransactionId { get; }

        /// <summary>
        /// 是否为顶层事务
        /// </summary>
        public bool IsTopTransaction { get; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 顶层事务是否dispose
        /// </summary>
        bool IsTopDisposed { get; set; } = false;

        /// <summary>
        /// 顶层事务是否已回滚
        /// </summary>
        bool IsTopRollback { get; set; } = false;

        /// <summary>
        /// 完成事务
        /// </summary>
        public void Commit()
        {
            if (IsTopTransaction && !IsTopRollback && !IsTopDisposed)
            {
                TopDbContextTransaction.Commit();
            }
            IsCompleted = true;
        }

        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (IsTopTransaction && !IsTopDisposed)
            {
                TopDbContextTransaction.Dispose();
                IsTopDisposed = true;
            }
            IsCompleted = true;
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void Rollback()
        {
            if (!IsTopRollback && !IsTopDisposed)
            {
                TopDbContextTransaction.Rollback();
                IsTopRollback = true;
                var tran = (TopDbContextTransaction as EfDbContextTransaction);
                if (tran != null)
                    tran.IsCompleted = true;
            }
            IsCompleted = true;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (IsTopTransaction && !IsTopRollback && !IsTopDisposed)
            {
                await TopDbContextTransaction.CommitAsync();
            }
            IsCompleted = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (!IsTopRollback && !IsTopDisposed)
            {
                await TopDbContextTransaction.RollbackAsync();
                IsTopRollback = true;
                var tran = (TopDbContextTransaction as EfDbContextTransaction);
                if (tran != null)
                    tran.IsCompleted = true;
            }
            IsCompleted = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (IsTopTransaction && !IsTopDisposed)
            {
                await TopDbContextTransaction.DisposeAsync();
                IsTopDisposed = true;
            }
            IsCompleted = true;
        }
    }
}