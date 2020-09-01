using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedType.Global
// ReSharper disable IdentifierTypo

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务包装类
    /// </summary>
    public class DefaultEfNestedTransactionnWrapper : IEfNestedTransactionWrapper
    {
        ConcurrentDictionary<DbContext, ConcurrentBag<TransactionModel>> Trans { get; }
            = new ConcurrentDictionary<DbContext, ConcurrentBag<TransactionModel>>();

        public virtual IDbContextTransaction GetCurrent(DbContext dbContext)
        {
            return Trans.TryGetValue(dbContext, out var list)
                ? list.FirstOrDefault()?.ScopedTransaction
                : null;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="beginTransactionFunc"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual IDbContextTransaction Begin(DbContext dbContext,
            Func<DbContext, IDbContextTransaction> beginTransactionFunc)
        {
            if (Trans.TryGetValue(dbContext, out var list))
            {
                var last = list.LastOrDefault(r =>
                    r.ScopedTransaction.IsTopTransaction && !r.ScopedTransaction.IsCompleted);
                if (last != null)
                {
                    // 存在该类型的事务
                    var tranModel = new TransactionModel
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
                var tran = beginTransactionFunc == null
                    ? dbContext.Database.BeginTransaction()
                    : beginTransactionFunc(dbContext);

                var top = new InnerEfDbContextTransaction(tran, true);
                // 不存在该类型的事务
                var tranModel = new TransactionModel
                {
                    DbContext = dbContext,
                    TopEfTransaction = top,
                    ScopedTransaction = top
                };

                if (!Trans.TryAdd(dbContext, new ConcurrentBag<TransactionModel> {tranModel}))
                    throw new Exception($"begin transaction error.");
                return tranModel.ScopedTransaction;
            }
        }

        /// <summary>
        /// scoped销毁时 dispose所有的事务
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var item in Trans)
            foreach (var transaction in item.Value)
                transaction?.ScopedTransaction?.Dispose();
        }

        public virtual IDbContextTransaction Begin(DbContext dbContext)
        {
            return Begin(dbContext, null);
        }
    }
}