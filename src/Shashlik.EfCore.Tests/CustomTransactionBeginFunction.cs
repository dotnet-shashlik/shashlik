// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Storage;
//
// namespace Shashlik.EfCore.Tests
// {
//     public class CustomTransactionBeginFunction : IEfNestedTransactionBeginFunction<TestDbContext1>
//     {
//         public Task<IDbContextTransaction> BeginTransactionAsync(DbContext dbContext)
//         {
//             return dbContext.Database.BeginTransactionAsync();
//         }
//     }
// }