using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务
    /// </summary>
    public interface IEfNestedTransactionWrapper : IDisposable
    {
        /// <summary>
        /// 获取当前进行的事务
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <returns></returns>
        IDbContextTransaction GetCurrent<TDbContext>(TDbContext dbContext) where TDbContext : DbContext;

        /// <summary>
        /// 开始事务,自定义开启方式
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <param name="isolationLevel">事务隔离级别</param>
        /// <param name="beginTransactionMethod">自定义开始事务的方法</param>
        /// <returns></returns>
        IDbContextTransaction Begin<TDbContext>(TDbContext dbContext,
            IsolationLevel? isolationLevel = null,
            Func<TDbContext, IDbContextTransaction> beginTransactionMethod = null) where TDbContext : DbContext;
    }
}