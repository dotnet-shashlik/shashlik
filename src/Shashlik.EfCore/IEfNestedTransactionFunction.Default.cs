using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 定义开启事务的方式
    /// </summary>
    public class DefaultEfNestedTransactionFunction<TDbContext> : IEfNestedTransactionFunction<TDbContext>
           where TDbContext : DbContext
    {
        public DefaultEfNestedTransactionFunction(Func<TDbContext, IDbContextTransaction> beginTransactionFunc)
        {
            BeginTransactionFunc = beginTransactionFunc;
        }

        /// <summary>
        /// 开启事务的方式
        /// </summary>
        public Func<TDbContext, IDbContextTransaction> BeginTransactionFunc { get; }
    }
}
