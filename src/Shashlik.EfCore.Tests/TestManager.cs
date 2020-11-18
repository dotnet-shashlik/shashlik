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
        public TestManager(TestDbContext1 dbContext, IEfNestedTransaction<TestDbContext1> transaction)
        {
            DbContext = dbContext;
            Transaction = transaction;
        }

        public TestDbContext1 DbContext { get; }
        public IEfNestedTransaction<TestDbContext1> Transaction { get; }

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

            DbContext.AddRange(roles.Select(r => new Roles
            {
                UserId = user.Id,
                Name = r
            }));
            await DbContext.SaveChangesAsync();
        }

        public virtual async Task CreateUserWithEfTransaction(string name, IEnumerable<string> roles, bool throwEx)
        {
            using var tran = DbContext.Database.BeginTransaction();

            try
            {
                var user = new Users
                {
                    Name = name,
                };

                DbContext.Add(user);
                await DbContext.SaveChangesAsync();

                if (throwEx) throw new Exception("事务应该回滚");

                DbContext.AddRange(roles.Select(r => new Roles
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
                throw;
            }
        }

        public virtual async Task CreateUserWithEfNestTransaction(string name, IEnumerable<string> roles, bool throwEx)
        {
            using var tran = Transaction.Begin();
            try
            {
                var user = new Users
                {
                    Name = name,
                };

                DbContext.Add(user);
                await DbContext.SaveChangesAsync();

                if (throwEx) throw new Exception("事务应该回滚");

                DbContext.AddRange(roles.Select(r => new Roles
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
                throw;
            }
        }

        public virtual void ClearDatas()
        {
            DbContext.Database.ExecuteSqlRaw("DELETE FROM `ROLES`");
            DbContext.Database.ExecuteSqlRaw("DELETE FROM `USERS`");
        }
    }
}