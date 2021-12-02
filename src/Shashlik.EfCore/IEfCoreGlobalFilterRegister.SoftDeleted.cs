using Shashlik.Kernel.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.EfCore
{
    /// <summary>
    /// <see cref="ISoftDeleted"/>软删除过滤器注册
    /// </summary>
    [Singleton]
    public class EfCoreGlobalFilterRegisterSoftDeleted : IEfCoreGlobalFilterRegister<ISoftDeleted>
    {
        public Expression<Func<TEntity, bool>> HasQueryFilter<TEntity>() where TEntity : ISoftDeleted
        {
            return (TEntity r) => !r.IsDeleted;
        }
    }
}
