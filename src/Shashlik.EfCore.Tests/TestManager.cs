using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shashlik.EfCore.Tests.Entities;
using Shashlik.EfCore.Transactional;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Tests
{
    [Transient]
    public class TestManager
    {
        public TestManager(TestDbContext1 dbContext)
        {
            DbContext = dbContext;
        }

        public TestDbContext1 DbContext { get; }

        [Transactional]
        public virtual async Task CreateUser(string name, IEnumerable<string> roles, bool throwEx)
        {
            var user = new Users
            {
                Name = name,
            };

            DbContext.Add(user);
            await DbContext.SaveChangesAsync();

            if (throwEx) throw new Exception("事务应该回滚");

            await DbContext.AddRangeAsync(roles.Select(r => new Roles
            {
                UserId = user.Id,
                Name = r
            }));
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task CreateUserWithEfTransaction(string name, IEnumerable<string> roles, bool throwEx)
        {
            await using var tran = await DbContext.Database.BeginTransactionAsync();

            try
            {
                var user = new Users
                {
                    Name = name,
                };

                DbContext.Add(user);
                await DbContext.SaveChangesAsync();

                if (throwEx) throw new Exception("事务应该回滚");

                await DbContext.AddRangeAsync(roles.Select(r => new Roles
                {
                    UserId = user.Id,
                    Name = r
                }));
                await DbContext.SaveChangesAsync();

                await tran.CommitAsync();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
            }
        }

        public virtual async Task CreateUserWithEfNestTransaction(string name, IEnumerable<string> roles, bool rollback)
        {
            var tran11 = DbContext.GetCurrentNestedTransaction();
            await using var tran = DbContext.BeginNestedTransaction();
            try
            {
                var user = new Users
                {
                    Name = name,
                };

                DbContext.Add(user);
                await DbContext.SaveChangesAsync();

                if (rollback) throw new Exception("事务应该回滚");

                await DbContext.AddRangeAsync(roles.Select(r => new Roles
                {
                    UserId = user.Id,
                    Name = r
                }));
                await DbContext.SaveChangesAsync();

                await tran.CommitAsync();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
            }
        }

        [TransactionScope]
        public virtual async Task CreateUserByTransactionScope(string name, IEnumerable<string> roles, bool throwEx)
        {
            var user = new Users
            {
                Name = name,
            };

            DbContext.Add(user);
            await DbContext.SaveChangesAsync();

            if (throwEx) throw new Exception("事务应该回滚");

            await DbContext.AddRangeAsync(roles.Select(r => new Roles
            {
                UserId = user.Id,
                Name = r
            }));
            await DbContext.SaveChangesAsync();
        }

        public virtual void ClearDatas()
        {
            DbContext.Database.ExecuteSqlRaw("DELETE FROM `ROLES`");
            DbContext.Database.ExecuteSqlRaw("DELETE FROM `USERS`");
        }
    }
}