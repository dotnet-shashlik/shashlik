using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    public interface IEfNestedTransactionBeginFunction
    {
        Task<IDbContextTransaction> BeginTransactionAsync(DbContext dbContext);
    }

    /// <summary>
    /// 定义开启事务的方式,不定义就使用默认的DbContext.Database.BeginTransaction(), 需注册为单例
    /// </summary>
    public interface IEfNestedTransactionBeginFunction<TDbContext> : IEfNestedTransactionBeginFunction
        where TDbContext : DbContext
    {
    }
}