using Microsoft.EntityFrameworkCore.Storage;
using Shashlik.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef本地环境事务,可以支持事务嵌套(虽然实际仍然是一个事务)
    /// </summary>
    public interface IEfTransaction : IScoped, IDisposable
    {
        /// <summary>
        /// 获取当前进行的事务
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <returns></returns>
        IDbContextTransaction GetCurrent(DbContext dbContext);

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <returns></returns>
        IDbContextTransaction Begin(DbContext dbContext);

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <param name="beginTransactionFunc">自定义开始事务的方法</param>
        /// <returns></returns>
        IDbContextTransaction Begin(DbContext dbContext, Func<DbContext, IDbContextTransaction> beginTransactionFunc);
    }
}