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
    /// ef嵌套事务
    /// </summary>
    public interface IEfNestedTransactionWrapper : IDisposable
    {
        /// <summary>
        /// 获取当前进行的事务
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <returns></returns>
        IDbContextTransaction GetCurrent(DbContext dbContext);

        /// <summary>
        /// 开始事务,默认DbContext的开启事务方式
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <returns></returns>
        IDbContextTransaction Begin(DbContext dbContext);

        /// <summary>
        /// 开始事务,自定义开启方式
        /// </summary>
        /// <param name="dbContext">上下文对象</param>
        /// <param name="beginTransactionFunc">自定义开始事务的方法</param>
        /// <returns></returns>
        IDbContextTransaction Begin(DbContext dbContext, Func<DbContext, IDbContextTransaction> beginTransactionFunc);
    }
}