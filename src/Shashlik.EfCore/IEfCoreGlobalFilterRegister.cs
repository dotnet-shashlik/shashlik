using System;
using System.Linq.Expressions;

// ReSharper disable TypeParameterCanBeVariant

namespace Shashlik.EfCore
{
    public interface IEfCoreGlobalFilterRegister
    {
    }

    /// <summary>
    /// EfCore全局过滤器注册器,需注册到服务容器<para></para>
    /// 建议根据过滤实际使用情况注册LifeTime类型<para></para>
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    public interface IEfCoreGlobalFilterRegister<TFilter> : IEfCoreGlobalFilterRegister
        where TFilter : class
    {
        public Expression<Func<TEntity, bool>> HasQueryFilter<TEntity>() where TEntity : TFilter;
    }
}