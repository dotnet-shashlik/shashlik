using System;
using System.Linq.Expressions;
using Shashlik.Kernel.Dependency;

namespace Shashlik.EfCore.Filter
{
    /// <summary>
    /// <see cref="ISoftDeleted"/>软删除过滤器注册
    /// </summary>
    [Singleton]
    public class EfCoreGlobalFilterRegisterSoftDeleted : IEfCoreGlobalFilterRegister<ISoftDeleted>
    {
        public Expression<Func<TEntity, bool>> HasQueryFilter<TEntity>() where TEntity : ISoftDeleted
        {
            return r => !r.IsDeleted;
        }
    }
}