using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore.NestedTransaction
{
    internal class ShashlikDbContextTransaction : IDbContextTransaction
    {
        private volatile bool _isRollback;
        private volatile bool _isDispose;
        private volatile bool _isCommit;
        private readonly Action? _disposedAction;

        public ShashlikDbContextTransaction(IDbContextTransaction topTransaction, bool isTop, Action? disposedAction = null)
        {
            TopTransaction = topTransaction;
            TransactionId = topTransaction.TransactionId;
            IsTop = isTop;
            _disposedAction = disposedAction;
        }

        public Guid TransactionId { get; }

        /// <summary>
        /// 是否已dispose
        /// </summary>
        public bool IsDispose
        {
            get => _isDispose;
            private set => _isDispose = value;
        }

        /// <summary>
        /// 是否已提交
        /// </summary>
        public bool IsCommit
        {
            get => _isCommit;
            private set => _isCommit = value;
        }

        /// <summary>
        /// 是否已回滚
        /// </summary>
        public bool IsRollback
        {
            get => _isRollback;
            private set => _isRollback = value;
        }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone => IsDispose || IsCommit || IsRollback;

        /// <summary>
        /// 是否为顶层事务
        /// </summary>
        public bool IsTop { get; }

        /// <summary>
        /// 原始/顶层事务数据
        /// </summary>
        private IDbContextTransaction? TopTransaction { get; set; }

        private void Valid()
        {
            if (TopTransaction is ShashlikDbContextTransaction shashlikDbContextTransaction
                && shashlikDbContextTransaction.IsDone)
                throw new InvalidOperationException("Top transaction has been done");
            if (IsDone)
                throw new InvalidOperationException("Already committed or rolled back.");
        }

        public void Dispose()
        {
            if (IsTop)
                TopTransaction!.Dispose();

            TopTransaction = null;
            IsDispose = true;
            _disposedAction?.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            if (IsTop)
                await TopTransaction!.DisposeAsync();

            TopTransaction = null;
            IsDispose = true;
        }

        public void Commit()
        {
            Valid();
            if (IsTop)
                TopTransaction!.Commit();
            IsCommit = true;
        }


        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Valid();
            if (IsTop)
                await TopTransaction!.CommitAsync(cancellationToken);
            IsCommit = true;
        }

        public void Rollback()
        {
            if (!IsRollback)
                TopTransaction!.RollbackAsync();
            IsRollback = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (!IsRollback)
                await TopTransaction!.RollbackAsync(cancellationToken);
            IsRollback = true;
        }
    }
}