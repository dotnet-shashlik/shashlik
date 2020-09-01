using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Shashlik.EfCore
{
    class InnerEfDbContextTransaction : IDbContextTransaction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topDbContextTransaction">顶层事务</param>
        /// <param name="isTopTransaction">是否为顶层事务</param>
        public InnerEfDbContextTransaction(
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
                if (TopDbContextTransaction is InnerEfDbContextTransaction tran)
                    tran.IsCompleted = true;
            }
            IsCompleted = true;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (IsTopTransaction && !IsTopRollback && !IsTopDisposed)
            {
                await TopDbContextTransaction.CommitAsync(cancellationToken);
            }
            IsCompleted = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (!IsTopRollback && !IsTopDisposed)
            {
                await TopDbContextTransaction.RollbackAsync(cancellationToken);
                IsTopRollback = true;
                if (TopDbContextTransaction is InnerEfDbContextTransaction tran)
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