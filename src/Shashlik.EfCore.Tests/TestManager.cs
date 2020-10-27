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
        public TestManager(TestDbContext dbContext)
        {
            DbContext = dbContext;
        }

        private TestDbContext DbContext { get; }

        // [Transactional]
        public async Task CreateUser(string name, IEnumerable<string> roles)
        {
            var user = new Users
            {
                Name = name,
            };

            DbContext.Add(user);
            await DbContext.SaveChangesAsync();

            DbContext.AddRange(roles.Select(r => new Roles
            {
                UserId = user.Id,
                Name = r
            }));
            await DbContext.SaveChangesAsync();
        }
    }
}