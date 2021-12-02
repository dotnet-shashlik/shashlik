using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Shashlik.EfCore
{
    public interface IEfCoreGlobalFilterRegister { }

    /// <summary>
    /// EfCore全局过滤器注册器,需注册到服务容器<para></para>
    /// 建议根据过滤实际使用情况注册LifeTime类型<para></para>
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    public interface IEfCoreGlobalFilterRegister<TFilter> : IEfCoreGlobalFilterRegister
    {
        public Expression<Func<TEntity, bool>> HasQueryFilter<TEntity>() where TEntity : TFilter;
    }
}
