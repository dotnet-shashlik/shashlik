using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 定义开启事务的方式,不定义就使用默认的DbContext.Database.BeginTransaction(),已自动注册为单例
    /// </summary>
    public interface IEfNestedTransactionBeginFunction<in TDbContext> : Shashlik.Kernel.Dependency.ISingleton
        where TDbContext : DbContext
    {
        IDbContextTransaction BeginTransaction(TDbContext dbContext);
    }
}