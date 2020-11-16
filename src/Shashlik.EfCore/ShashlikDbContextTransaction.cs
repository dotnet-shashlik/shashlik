using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    internal class ShashlikDbContextTransaction : IDbContextTransaction
    {
        public ShashlikDbContextTransaction(IDbContextTransaction topTransaction)
        {
            TopTransaction = topTransaction;
        }

        /// <summary>
        /// 顶层事务是否已dispose
        /// </summary>
        public bool IsDispose { get; private set; }

        /// <summary>
        /// 顶层事务是否已提交
        /// </summary>
        public bool IsCommit { get; private set; }

        /// <summary>
        /// 顶层事务是否已回滚
        /// </summary>
        public bool IsRollback { get; private set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone => IsDispose || IsCommit || IsRollback;

        /// <summary>
        /// 是否为顶层事务
        /// </summary>
        public bool IsTop => !(TopTransaction is ShashlikDbContextTransaction);

        /// <summary>
        /// 原始事务数据
        /// </summary>
        private IDbContextTransaction TopTransaction { get; }

        private void Valid()
        {
            if (TopTransaction is ShashlikDbContextTransaction shashlikDbContextTransaction
                && shashlikDbContextTransaction.IsDone)
                throw new TransactionException("Top transaction has been done.");
            if (IsDone)
                throw new TransactionException("Top transaction has been done.");
        }

        public void Dispose()
        {
            if (IsTop)
                TopTransaction.Dispose();

            IsDispose = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (IsTop)
                await TopTransaction.DisposeAsync();

            IsDispose = true;
        }

        public void Commit()
        {
            Valid();
            if (IsTop)
                TopTransaction.Commit();
            IsCommit = true;
        }


        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Valid();
            if (IsTop)
                await TopTransaction.CommitAsync(cancellationToken);
            IsCommit = true;
        }

        public void Rollback()
        {
            if (!IsRollback)
                TopTransaction.Rollback();
            IsRollback = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (!IsRollback)
                await TopTransaction.RollbackAsync(cancellationToken);
            IsRollback = true;
        }

        public Guid TransactionId => TopTransaction.TransactionId;
    }
}