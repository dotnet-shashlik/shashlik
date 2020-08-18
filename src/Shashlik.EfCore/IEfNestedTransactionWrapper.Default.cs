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
using System.Threading;

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务包装类
    /// </summary>
    public class DefaultEfNestedTransactionnWrapper : IEfNestedTransactionWrapper
    {

        ConcurrentDictionary<DbContext, ConcurrentBag<TransationModel>> trans { get; }
            = new ConcurrentDictionary<DbContext, ConcurrentBag<TransationModel>>();

        public virtual IDbContextTransaction GetCurrent(DbContext dbContext)
        {
            if (trans.TryGetValue(dbContext, out var list))
            {
                return list.FirstOrDefault()?.ScopedTransaction;
            }
            return null;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="capPublisher"></param>
        /// <returns></returns>
        public virtual IDbContextTransaction Begin(DbContext dbContext, Func<DbContext, IDbContextTransaction> beginTransactionFunc)
        {
            if (trans.TryGetValue(dbContext, out var list))
            {
                var last = list.LastOrDefault(r => r.ScopedTransaction.IsTopTransaction && !r.ScopedTransaction.IsCompleted);
                if (last != null)
                {
                    // 存在该类型的事务
                    var tranModel = new TransationModel
                    {
                        DbContext = dbContext,
                        TopEfTransaction = last.TopEfTransaction,
                        ScopedTransaction = new InnerEfDbContextTransaction(last.TopEfTransaction, false)
                    };
                    list.Add(tranModel);
                    return tranModel.ScopedTransaction;
                }
            }

            {
                IDbContextTransaction tran;
                if (beginTransactionFunc == null)
                    tran = dbContext.Database.BeginTransaction();
                else
                    tran = beginTransactionFunc(dbContext);

                var top = new InnerEfDbContextTransaction(tran, true);
                // 不存在该类型的事务
                var tranModel = new TransationModel
                {
                    DbContext = dbContext,
                    TopEfTransaction = top,
                    ScopedTransaction = top
                };

                if (!trans.TryAdd(dbContext, new ConcurrentBag<TransationModel> { tranModel }))
                    throw new Exception($"begin transaction error.");
                return tranModel.ScopedTransaction;
            }
        }

        /// <summary>
        /// scoped销毁时 dispose所有的事务
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var item in trans)
                foreach (var transation in item.Value)
                    transation?.ScopedTransaction?.Dispose();
        }

        public virtual IDbContextTransaction Begin(DbContext dbContext)
        {
            return Begin(dbContext, null);
        }
    }
}