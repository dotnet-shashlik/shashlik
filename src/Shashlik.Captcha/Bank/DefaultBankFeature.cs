using Guc.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guc.Features.Bank
{
    class DefaultBankFeature : IBankFeature
    {
        public DefaultBankFeature(FeatureDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public FeatureDbContext DbContext { get; }
        public async Task<List<Banks>> Get()
        {
            return await DbContext.Set<Banks>().ToListAsync();
        }

        public async Task<Banks> Get(string bankId)
        {
            return await DbContext.Set<Banks>().FirstOrDefaultAsync(r => r.Id == bankId);
        }

        public async Task Insert(Banks bank)
        {
            if (bank == null)
            {
                throw new ArgumentNullException(nameof(bank));
            }

            await DbContext.AddAsync(bank);
            await DbContext.SaveChangesAsync();
        }

        public async Task Insert(IEnumerable<Banks> banks)
        {
            if (banks.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(banks));
            }

            await DbContext.AddRangeAsync(banks);
            await DbContext.SaveChangesAsync();
        }
    }
}
