using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Mapper
{
    /// <summary>
    /// 单向 ,默认配置(直接继承接口有效,注意继承时的查询bug)
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public interface IMapFrom<TSource>
    {
    }

    /// <summary>
    /// 单向映射,可自定义配置(直接继承接口有效,注意继承时的查询bug)
    /// </summary>
    /// <typeparam name="TDest"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    public interface IMapFrom<TSource, TDest>
    {
        /// <summary>
        /// 自定义配置映射
        /// </summary>
        /// <param name="mappingExpression"></param>
        void Config(IMappingExpression<TSource, TDest> mapper);
    }

    /// <summary>
    /// 单向映射,默认配置(直接继承接口有效,注意继承时的查询bug)
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    public interface IMapTo<TDest>
    {

    }

    /// <summary>
    /// 单向映射,可自定义配置(直接继承接口有效,注意继承时的查询bug)
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    public interface IMapTo<TDest, TSource>
    {
        /// <summary>
        /// 自定义配置映射
        /// </summary>
        /// <param name="mappingExpression"></param>
        void Config(IMappingExpression<TSource, TDest> mapper);
    }
}
