using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 定义开启事务的方式
    /// </summary>
    public interface IEfNestedTransactionFunction<in TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// 开启事务的方式
        /// </summary>
        Func<TDbContext, IDbContextTransaction> BeginTransactionFunc { get; }
    }
}
