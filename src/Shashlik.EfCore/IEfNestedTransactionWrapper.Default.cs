using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Data;
using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedType.Global
// ReSharper disable IdentifierTypo

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务包装类
    /// </summary>
    public class DefaultEfNestedTransactionWrapper : IEfNestedTransactionWrapper
    {
        private ConcurrentDictionary<DbContext, ConcurrentBag<TransactionModel>> Trans { get; }
            = new ConcurrentDictionary<DbContext, ConcurrentBag<TransactionModel>>();

        public virtual IDbContextTransaction GetCurrent<TDbContext>(TDbContext dbContext)
            where TDbContext : DbContext
        {
            return Trans.TryGetValue(dbContext, out var list)
                ? list.FirstOrDefault()?.NestTransaction
                : null;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="isolationLevel"></param>
        /// <param name="beginTransactionMethod"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual IDbContextTransaction Begin<TDbContext>(TDbContext dbContext,
            IsolationLevel? isolationLevel = null,
            Func<TDbContext, IDbContextTransaction> beginTransactionMethod = null) where TDbContext : DbContext
        {
            if (Trans.TryGetValue(dbContext, out var list))
            {
                var last = list.LastOrDefault(r =>
                    r.NestTransaction.IsTopTransaction && !r.NestTransaction.IsCompleted);
                if (last != null)
                {
                    // 存在该类型的事务
                    var tranModel = new TransactionModel
                    {
                        DbContext = dbContext,
                        TopEfTransaction = last.TopEfTransaction,
                        NestTransaction = new InnerEfDbContextTransaction(last.TopEfTransaction, false)
                    };
                    list.Add(tranModel);
                    return tranModel.NestTransaction;
                }
            }

            {
                var tran = beginTransactionMethod == null
                    ? (isolationLevel.HasValue
                        ? dbContext.Database.BeginTransaction(isolationLevel.Value)
                        : dbContext.Database.BeginTransaction())
                    : beginTransactionMethod(dbContext);

                var top = new InnerEfDbContextTransaction(tran, true);
                // 不存在该类型的事务
                var tranModel = new TransactionModel
                {
                    DbContext = dbContext,
                    TopEfTransaction = top,
                    NestTransaction = top
                };

                if (list == null && !Trans.TryAdd(dbContext, new ConcurrentBag<TransactionModel> {tranModel}))
                    throw new Exception($"begin transaction error.");
                if (list != null)
                    list!.Add(tranModel);
                return tranModel.NestTransaction;
            }
        }

        /// <summary>
        /// scoped销毁时 dispose所有的事务
        /// </summary>
        public virtual void Dispose()
        {
            foreach (var item in Trans)
            foreach (var transaction in item.Value)
                transaction?.NestTransaction?.Dispose();
            Trans.Clear();
        }
    }
}