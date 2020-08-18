﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shashlik.EfCore
{
    /// <summary>
    /// ef嵌套事务
    /// </summary>
    public interface IEfNestedTransaction<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// 当前事务
        /// </summary>
        IDbContextTransaction Current { get; }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction Begin();
    }
}