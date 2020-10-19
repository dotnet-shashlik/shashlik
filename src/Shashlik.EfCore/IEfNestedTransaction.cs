using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务
    /// </summary>
    public interface IEfNestedTransaction
    {
        /// <summary>
        /// 当前事务
        /// </summary>
        IDbContextTransaction Current { get; }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别,null使用默认的级别</param>
        /// <returns></returns>
        IDbContextTransaction Begin(IsolationLevel? isolationLevel = null);
    }

    /// <summary>
    /// ef嵌套事务
    /// </summary>
    public interface IEfNestedTransaction<TDbContext> : IEfNestedTransaction
        where TDbContext : DbContext
    {
    }
}