using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore.Tests
{
    public class CustomTransactionBeginFunction : IEfNestedTransactionBeginFunction<TestDbContext1>
    {
        public Task<IDbContextTransaction> BeginTransactionAsync(TestDbContext1 dbContext)
        {
            return dbContext.Database.BeginTransactionAsync();
        }
    }
}