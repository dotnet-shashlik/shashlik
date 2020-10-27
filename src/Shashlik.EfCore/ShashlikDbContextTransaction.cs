using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    internal class ShashlikDbContextTransaction<TDbContext> : IDbContextTransaction
        where TDbContext : DbContext
    {
        public ShashlikDbContextTransaction(TDbContext dbContext, IDbContextTransaction transaction)
        {
            DbContext = dbContext;
            Transaction = transaction;
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary>
        /// 上下文
        /// </summary>
        public TDbContext DbContext { get; }

        /// <summary>
        /// 原始事务数据
        /// </summary>
        private IDbContextTransaction Transaction { get; }

        public void Dispose()
        {
            Transaction.Dispose();
            IsDone = true;
        }

        public async ValueTask DisposeAsync()
        {
            await Transaction.DisposeAsync();
            IsDone = true;
        }

        public void Commit()
        {
            Transaction.Commit();
            IsDone = true;
        }

        public void Rollback()
        {
            Transaction.Rollback();
            IsDone = true;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await Transaction.CommitAsync(cancellationToken);
            IsDone = true;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await Transaction.RollbackAsync(cancellationToken);
            IsDone = true;
        }

        public Guid TransactionId => Transaction.TransactionId;
    }
}