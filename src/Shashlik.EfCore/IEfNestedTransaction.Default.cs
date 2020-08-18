using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 默认的环境事务
    /// </summary>
    public class DefaultEfNestedTransaction<TDbContext> : IEfNestedTransaction<TDbContext>
           where TDbContext : DbContext
    {
        public DefaultEfNestedTransaction(
            IEfNestedTransactionWrapper efTransaction,
            TDbContext dbContext)
        {
            EfTransaction = efTransaction;
            DbContext = dbContext;
        }

        IEfNestedTransactionWrapper EfTransaction { get; }
        TDbContext DbContext { get; set; }

        public virtual IDbContextTransaction Current => EfTransaction.GetCurrent(DbContext);

        public virtual IDbContextTransaction Begin()
        {
            return EfTransaction.Begin(DbContext);
        }
    }
}
