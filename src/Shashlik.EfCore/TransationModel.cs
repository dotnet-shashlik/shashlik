using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.EfCore
{
    /// <summary>
    /// 事务模型
    /// </summary>
    class TransationModel
    {
        public DbContext DbContext { get; set; }

        public IDbContextTransaction TopEfTransaction { get; set; }

        public InnerEfDbContextTransaction ScopedTransaction { get; set; }
    }
}
