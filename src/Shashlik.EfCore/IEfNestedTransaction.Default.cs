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
        public DefaultEfNestedTransaction(IEfNestedTransactionWrapper efTransactionWrapper, TDbContext dbContext)
        {
            EfTransactionWrapper = efTransactionWrapper;
            DbContext = dbContext;
            // 从服务中获取开启事务的方式,没注册的话就是null
            BeginTransactionMethod = dbContext.GetService<IEfNestedTransactionMethod<TDbContext>>();
        }

        /// <summary>
        /// 嵌套事务包装类
        /// </summary>
        private IEfNestedTransactionWrapper EfTransactionWrapper { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private IEfNestedTransactionMethod<TDbContext> BeginTransactionMethod { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public virtual IDbContextTransaction Current => EfTransactionWrapper.GetCurrent(DbContext);

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin()
        {
            return BeginTransactionMethod == null
                ? EfTransactionWrapper.Begin(DbContext)
                : EfTransactionWrapper.Begin(DbContext, BeginTransactionMethod.BeginTransaction);
        }
    }
}