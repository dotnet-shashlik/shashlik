using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace Shashlik.EfCore
{
    /// <summary>
    /// 默认的嵌套事务
    /// </summary>
    public class DefaultEfNestedTransaction<TDbContext> : IEfNestedTransaction<TDbContext>
        where TDbContext : DbContext
    {
        public DefaultEfNestedTransaction(IEfNestedTransactionWrapper efTransaction, TDbContext dbContext)
        {
            EfTransaction = efTransaction;
            DbContext = dbContext;
            // 从服务中获取开启事务的方式,没注册的话就是null
            BeginTransactionFunc = dbContext.GetService<IEfNestedTransactionFunction<TDbContext>>();
        }

        /// <summary>
        /// 嵌套事务包装类
        /// </summary>
        private IEfNestedTransactionWrapper EfTransaction { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private IEfNestedTransactionFunction<TDbContext> BeginTransactionFunc { get; }

        public virtual IDbContextTransaction Current => EfTransaction.GetCurrent(DbContext);

        public virtual IDbContextTransaction Begin()
        {
            if (BeginTransactionFunc == null)
                return EfTransaction.Begin(DbContext);
            return EfTransaction.Begin(DbContext,
                dbContext => BeginTransactionFunc.BeginTransactionFunc((TDbContext) dbContext));
        }
    }
}