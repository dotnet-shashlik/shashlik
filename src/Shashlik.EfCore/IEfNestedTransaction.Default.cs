using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 默认的环境事务
    /// </summary>
    public class DefaultEfNestedTransaction<TDbContext> : IEfNestedTransaction<TDbContext>
           where TDbContext : DbContext
    {
        public DefaultEfNestedTransaction(IEfNestedTransactionWrapper efTransaction, TDbContext dbContext, Func<TDbContext, IDbContextTransaction> beginTransactionFunc)
        {
            EfTransaction = efTransaction;
            DbContext = dbContext;
            BeginTransactionFunc = beginTransactionFunc;
        }

        IEfNestedTransactionWrapper EfTransaction { get; }
        TDbContext DbContext { get; }
        /// <summary>
        /// 开启事务的方式
        /// </summary>
        Func<TDbContext, IDbContextTransaction> BeginTransactionFunc { get; }

        public virtual IDbContextTransaction Current => EfTransaction.GetCurrent(DbContext);

        public virtual IDbContextTransaction Begin()
        {
            if (BeginTransactionFunc == null)
                return EfTransaction.Begin(DbContext);
            else
                return EfTransaction.Begin(DbContext, dbContext => BeginTransactionFunc((TDbContext)dbContext));
        }
    }
}
