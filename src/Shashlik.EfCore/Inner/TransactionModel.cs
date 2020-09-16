using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 事务模型
    /// </summary>
    internal class TransactionModel
    {
        public DbContext DbContext { get; set; }

        public IDbContextTransaction TopEfTransaction { get; set; }

        public InnerEfDbContextTransaction ScopedTransaction { get; set; }
    }
}
