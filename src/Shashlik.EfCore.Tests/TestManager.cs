using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shashlik.EfCore.Tests.Entities;
using Shashlik.EfCore.Transactional;

namespace Shashlik.EfCore.Tests
{
    public class TestManager : Kernel.Dependency.ITransient
    {
        public TestManager(TestDbContext dbContext, IEfNestedTransaction<TestDbContext> transaction)
        {
            DbContext = dbContext;
            Transaction = transaction;
        }

        public TestDbContext DbContext { get; }
        public IEfNestedTransaction<TestDbContext> Transaction { get; }

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
    }
}