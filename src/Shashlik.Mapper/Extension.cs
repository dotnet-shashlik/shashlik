using System.Linq;
using System.Reflection;
using AutoMapper;
using System;
using AutoMapper.QueryableExtensions;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Linq.Expressions;
using Shashlik.Utils.Extensions;
using System.Collections.Generic;
using Shashlik.Utils.Common;

namespace Shashlik.Mapper
{
    public static class Extensions
    {
        /// <summary>
        /// 增加auto mapper自动化映射,注册了全局单例IMapper
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelBuilder AddAutoMapperByConvention(this IKernelBuilder kernelBuilder)
        {
            var assemblies = AssemblyHelper.GetReferredAssemblies(typeof(IMapFrom<>).Assembly);
            return kernelBuilder.AddAutoMapperByConvention(assemblies);
        }

        /// <summary>
        /// 增加auto mapper自动化映射,注册了全局单例IMapper
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelBuilder AddAutoMapperByConvention(this IKernelBuilder kernelBuilder, DependencyContext dependencyContext)
        {
            if (dependencyContext == null)
            {
                throw new ArgumentNullException(nameof(dependencyContext));
            }

            var assemblies = AssemblyHelper.GetReferredAssemblies(typeof(IMapFrom<>).Assembly, dependencyContext);
            return kernelBuilder.AddAutoMapperByConvention(assemblies);
        }

        /// <summary>
        /// 增加auto mapper自动化映射,注册了全局单例IMapper
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="assemblies">需要注册的程序集</param>
        /// <returns></returns>
        public static IKernelBuilder AddAutoMapperByConvention(this IKernelBuilder kernelBuilder, IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }
            var configuration = new MapperConfiguration(config =>
            {
                var method = config.GetType().GetTypeInfo().GetMethods().Single(r => r.Name == "CreateMap" && r.IsGenericMethodDefinition && r.GetParameters().Length == 1);

                foreach (var assembly in assemblies)
                {
                    var iMapFrom1Types = assembly.DefinedTypes.Where(r => !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapFrom<>))).ToList();
                    var iMapFromTypes = assembly.DefinedTypes.Where(r => !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapFrom<,>))).ToList();
                    var iMapTo1Types = assembly.DefinedTypes.Where(r => !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapTo<>))).ToList();
                    var iMapToTypes = assembly.DefinedTypes.Where(r => !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapTo<,>))).ToList();

                    // IMapFrom<TFrom>
                    foreach (var item in iMapFrom1Types)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var fromTypes = interfaces
                                            .Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 1 && r.FullName.StartsWith(typeof(IMapFrom<>).FullName))
                                            .Select(r => r.GenericTypeArguments[0])
                                            .ToList();

                        foreach (var fromType in fromTypes)
                            config.CreateMap(fromType, item, MemberList.None);
                    }

                    // IMapFrom<TFrom, TDest>
                    foreach (var item in iMapFromTypes)
                    {
                        var interfaces = item.GetInterfaces(false);

                        var interfaceTypes = interfaces.Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 2 && r.FullName.StartsWith(typeof(IMapFrom<,>).FullName)).ToHashSet();
                        if (interfaceTypes.IsNullOrEmpty())
                            continue;

                        foreach (var interfaceType in interfaceTypes)
                        {
                            var fromType = interfaceType.GenericTypeArguments[0];
                            var destType = interfaceType.GenericTypeArguments[1];

                            var expression = method.MakeGenericMethod(fromType, destType).Invoke(config, new object[] { MemberList.None });
                            var expressionType = typeof(IMappingExpression<,>).MakeGenericType(fromType, destType);

                            var configMethod = item.GetMethods().First(r => r.Name == "Config" && r.GetParameters().Length == 1 && r.GetParameters().First().ParameterType == expressionType);

                            object obj = null;
                            try
                            {
                                obj = Activator.CreateInstance(item);
                            }
                            catch
                            {
                                throw new Exception($"auto mapper 初始化失败:无法创建类型:{item}");
                            }
                            if (obj == null)
                                throw new Exception($"auto mapper 初始化失败:无法创建类型:{item}");
                            try
                            {
                                configMethod.Invoke(obj, new object[] { expression });
                            }
                            catch
                            {
                                throw new Exception($"auto mapper 初始化失败:初始化类型[{item}]配置失败");
                            }
                            finally
                            {
                                var disposeObj = obj as IDisposable;
                                if (disposeObj != null)
                                    disposeObj.Dispose();
                            }
                        }
                    }

                    // IMapTo < TDest >
                    foreach (var item in iMapTo1Types)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var toTypes = interfaces
                                                .Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 1 && r.FullName.StartsWith(typeof(IMapTo<>).FullName))
                                                .Select(r => r.GenericTypeArguments[0])
                                                .ToList();
                        if (toTypes == null)
                            continue;
                        foreach (var toType in toTypes)
                            config.CreateMap(item, toType, MemberList.None);
                    }

                    // IMapTo<TDest, TSource>
                    foreach (var item in iMapToTypes)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var interfaceTypes = interfaces.Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 2 && r.FullName.StartsWith(typeof(IMapTo<,>).FullName)).ToList();
                        if (interfaceTypes.IsNullOrEmpty())
                            continue;
                        foreach (var interfaceType in interfaceTypes)
                        {
                            var destType = interfaceType.GenericTypeArguments[0];
                            var fromType = interfaceType.GenericTypeArguments[1];

                            var expression = method.MakeGenericMethod(fromType, destType).Invoke(config, new object[] { MemberList.None });
                            var expressionType = typeof(IMappingExpression<,>).MakeGenericType(fromType, destType);
                            var configMethod = item.GetMethods().First(r => r.Name == "Config" && r.GetParameters().Length == 1 && r.GetParameters().First().ParameterType == expressionType);
                            object obj = null;
                            try
                            {
                                obj = Activator.CreateInstance(item);
                            }
                            catch
                            {
                                throw new Exception($"auto mapper 初始化失败:无法创建类型:{item},请检查是否有无参构造函数.");
                            }
                            if (obj == null)
                                throw new Exception($"auto mapper 初始化失败:无法创建类型:{item},请检查是否有无参构造函数.");
                            try
                            {
                                configMethod.Invoke(obj, new object[] { expression });
                                var disposeObj = obj as IDisposable;
                                if (disposeObj != null)
                                    disposeObj.Dispose();
                            }
                            catch
                            {
                                throw new Exception($"auto mapper 初始化失败:初始化类型[{item}]配置失败");
                            }
                            finally
                            {
                                var disposeObj = obj as IDisposable;
                                if (disposeObj != null)
                                    disposeObj.Dispose();
                            }
                        }
                    }
                }
            });

            configuration.AssertConfigurationIsValid();
            var mapper = new AutoMapper.Mapper(configuration);
            ShashlikMapper.Instance = mapper;

            kernelBuilder.Services.AddSingleton<IMapper>(mapper);
            return kernelBuilder;
        }

        /*
         * AllFromPath说明:
         * 
         * 所有的成员从源对象的一个属性映射,例：                                                                                    
         * 目标类,用户dto,class:UserDto                                                                                                        
         * 　　　　　　　Name                                                                                                           
         *         Age                                                                                                            
         * 源类,用户角色表,包含用户属性    ,clas:UserRole                                                                                       
         *         User                                                                                                           
         * 用户表,class:User                                                                                                                 
         *         Id                                                                                                             
         *         Name                                                                                                           
         *         Age                                                                                                            
         *                                                                                                                        
         * 建立UserRole->UserDto的映射时,Name和Age当然无法自动映射,可以手动配置Name MapFrom(User.Name),但是属性太多时就很繁琐     
         * 此方法可以直接配置所有属性来源                                                                                         
         *                                                                                                                        
         * 如下配置:                                                                                                              
         *     mapper.AllFromPath(r=>r.User)                                                                                      
         * 即配置所有的属性来源User属性       
         * 
         * **/

        /// <summary>
        /// 所有的成员从源对象的一个属性映射
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <typeparam name="TPathProperty"></typeparam>
        /// <param name="pathAction"></param>
        /// <param name="propertySelector"></param>
        public static void AllFromPath<TSource, TDestination, TPathProperty>(this IMappingExpression<TSource, TDestination> pathAction, Expression<Func<TSource, TPathProperty>> propertySelector)
        {
            var proName = GetPropertyName(propertySelector);

            var ps = typeof(TDestination)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .Where(r => !r.GetIndexParameters().Any())
                .ToList();

            foreach (var item in ps)
            {
                var target = typeof(TPathProperty).GetTypeInfo();
                // 属性不存在,跳过
                if (!target.DeclaredProperties.Any(r => r.Name == item.Name && r.CanWrite))
                    continue;
                // 类型不一致 跳过
                if (target.GetProperty(item.Name).PropertyType != item.PropertyType)
                    continue;

                var exp = Expression.Lambda(Expression.Property(propertySelector.Body, item.Name), propertySelector.Parameters.First());
                var expType = exp.GetType();

                pathAction.ForMember(item.Name, r =>
                {
                    var eee2 = typeof(Expression<>)
                    .MakeGenericType(typeof(Func<,>)
                                        .MakeGenericType(typeof(TSource), item.PropertyType)
                    );

                    var mapFromMethod = r.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .Where(m => m.Name == "MapFrom")
                        .Where(m => m.IsGenericMethodDefinition)
                        .Where(m => m.GetGenericArguments().Length == 1)
                        .Where(m => m.GetParameters().Length == 1 && m.GetParameters().First().ParameterType.IsSubTypeOfGenericType(typeof(Expression<>)))
                        .SingleOrDefault();

                    if (mapFromMethod == null)
                        throw new Exception($"IMemberConfigurationExpression<TSource, TDestination, object>> [void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> mapExpression)] method not found.");

                    mapFromMethod.MakeGenericMethod(item.PropertyType).Invoke(r, new[] { exp });
                });
            }
        }

        /// <summary>
        /// 得到一个字段（Field）或属性（Property）名
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="express"></param>
        /// <returns></returns>
        static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> express)
        {
            if (express == null)
            {
                throw new ArgumentNullException("express");
            }
            MemberExpression memberExpression = express.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("请为类型 \"" + typeof(T).FullName + "\" 的指定一个字段（Field）或属性（Property）作为 Lambda 的主体（Body）。");
            }
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// 查询映射
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<TDest> QueryTo<TDest>(this IQueryable source)
        {
            return source.ProjectTo<TDest>(ShashlikMapper.Instance.ConfigurationProvider);
        }

        /// <summary>
        /// 对象映射 obj-><typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest MapTo<TDest>(this object obj)
        {
            return ShashlikMapper.Instance.Map<TDest>(obj);
        }

        /// <summary>
        /// 对象映射 obj-><typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static void MapTo<TSource, TDest>(this TSource obj, TDest destObj)
        {
            ShashlikMapper.Instance.Map(obj, destObj);
        }
    }
}