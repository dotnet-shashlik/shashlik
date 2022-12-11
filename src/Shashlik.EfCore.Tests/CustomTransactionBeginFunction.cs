using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shashlik.EfCore.NestedTransaction;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Tests
{
    [Singleton(IgnoreServiceType = new[] { typeof(IEfNestedTransactionBeginFunction) })]
    public class CustomTransactionBeginFunction : IEfNestedTransactionBeginFunction<TestDbContext1>
    {
        public Task<IDbContextTransaction> BeginTransactionAsync(DbContext dbContext)
        {
            return dbContext.Database.BeginTransactionAsync();
        }
    }
}