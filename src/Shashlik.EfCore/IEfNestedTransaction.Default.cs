using System;
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
        public DefaultEfNestedTransaction(
            IEfNestedTransactionWrapper efTransactionWrapper,
            IServiceProvider serviceProvider
        )
        {
            EfTransactionWrapper = efTransactionWrapper;
            DbContext = serviceProvider.GetRequiredService<TDbContext>();
            BeginFunction = serviceProvider.GetService<IEfNestedTransactionBeginFunction<TDbContext>>();
        }

        /// <summary>
        /// 嵌套事务包装类
        /// </summary>
        private IEfNestedTransactionWrapper EfTransactionWrapper { get; }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        private IEfNestedTransactionBeginFunction<TDbContext> BeginFunction { get; }

        /// <summary>
        /// 数据库上下文
        /// </summary>
        private TDbContext DbContext { get; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public virtual IDbContextTransaction Current => EfTransactionWrapper.GetCurrent(DbContext);

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin(IsolationLevel? isolationLevel = null)
        {
            return BeginFunction == null
                ? EfTransactionWrapper.Begin(DbContext, isolationLevel)
                : EfTransactionWrapper.Begin(DbContext, isolationLevel, BeginFunction.BeginTransaction);
        }
    }
}