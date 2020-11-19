using System.Linq;
using System.Reflection;
using AutoMapper;
using System;
using AutoMapper.QueryableExtensions;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Utils.Extensions;
using System.Collections.Generic;
using System.Data;
using Shashlik.Utils.Helpers;
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantExplicitArrayCreation

namespace Shashlik.AutoMapper
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 增加auto mapper自动化映射,注册了全局单例IMapper
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static IKernelServices AddAutoMapperByConvention(this IKernelServices kernelService,
            DependencyContext? dependencyContext = null)
        {
            var assemblies =
                ReflectionHelper.GetReferredAssemblies(typeof(AutoMapperExtensions).Assembly, dependencyContext);
            return kernelService.AddAutoMapperByConvention(assemblies);
        }

        /// <summary>
        /// 增加auto mapper自动化映射,注册了全局单例IMapper
        /// </summary>
        /// <param name="kernelService"></param>
        /// <param name="assemblies">需要注册的程序集</param>
        /// <returns></returns>
        public static IKernelServices AddAutoMapperByConvention(this IKernelServices kernelService,
            IEnumerable<Assembly> assemblies)
        {
            if (assemblies is null)
                throw new ArgumentNullException(nameof(assemblies));

            var configuration = new MapperConfiguration(config =>
            {
                var method = config.GetType().GetTypeInfo().GetMethod("CreateMap", new Type[] {typeof(MemberList)});
                if (method is null || !method.IsGenericMethodDefinition)
                    throw new MissingMethodException(nameof(IMapperConfigurationExpression), "CreateMap");
                //.Single(r => r.Name == "CreateMap" && r.IsGenericMethodDefinition && r.GetParameters().Length == 1);

                foreach (var assembly in assemblies)
                {
                    var iMapFrom1Types = assembly.DefinedTypes.Where(r =>
                        !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapFrom<>))).ToList();
                    var iMapFromTypes = assembly.DefinedTypes.Where(r =>
                        !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapFrom<,>))).ToList();
                    var iMapTo1Types = assembly.DefinedTypes
                        .Where(r => !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapTo<>))).ToList();
                    var iMapToTypes = assembly.DefinedTypes.Where(r =>
                        !r.IsAbstract && r.IsClass && r.IsSubTypeOfGenericType(typeof(IMapTo<,>))).ToList();

                    // IMapFrom<TFrom>
                    foreach (var item in iMapFrom1Types)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var fromTypes = interfaces
                            .Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 1 &&
                                        r.FullName!.StartsWith(typeof(IMapFrom<>).FullName!))
                            .Select(r => r.GenericTypeArguments[0])
                            .ToList();

                        foreach (var fromType in fromTypes)
                            config.CreateMap(fromType, item, MemberList.None);
                    }

                    // IMapFrom<TFrom, TDest>
                    foreach (var item in iMapFromTypes)
                    {
                        var interfaces = item.GetInterfaces(false);

                        var interfaceTypes = interfaces.Where(r =>
                            r.IsGenericType && r.GenericTypeArguments.Length == 2 &&
                            r.FullName!.StartsWith(typeof(IMapFrom<,>).FullName!)).ToHashSet();
                        if (interfaceTypes.IsNullOrEmpty())
                            continue;

                        foreach (var interfaceType in interfaceTypes)
                        {
                            var fromType = interfaceType.GenericTypeArguments[0];
                            var destType = interfaceType.GenericTypeArguments[1];

                            var expression = method.MakeGenericMethod(fromType, destType)
                                .Invoke(config, new object[] {MemberList.None});
                            var expressionType = typeof(IMappingExpression<,>).MakeGenericType(fromType, destType);

                            var configMethod = item.GetMethods().First(r =>
                                r.Name == "Config" && r.GetParameters().Length == 1 &&
                                r.GetParameters().First().ParameterType == expressionType);

                            object obj;
                            try
                            {
                                obj = Activator.CreateInstance(item);
                            }
                            catch
                            {
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] can not create instance of {item}, must contains non-argument constructor.");
                            }

                            if (obj is null)
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] can not create instance of {item}, must contains non-argument constructor.");
                            try
                            {
                                configMethod.Invoke(obj, new object[] {expression});
                                using (obj as IDisposable)
                                {
                                }
                            }
                            catch (Exception e)
                            {
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] create mapper {fromType}->{destType} error on type {item}.", e);
                            }
                        }
                    }

                    // IMapTo < TDest >
                    foreach (var item in iMapTo1Types)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var toTypes = interfaces
                            .Where(r => r.IsGenericType && r.GenericTypeArguments.Length == 1 &&
                                        r.FullName!.StartsWith(typeof(IMapTo<>).FullName!))
                            .Select(r => r.GenericTypeArguments[0])
                            .ToList();
                        if (toTypes.IsNullOrEmpty())
                            continue;
                        foreach (var toType in toTypes)
                            config.CreateMap(item, toType, MemberList.None);
                    }

                    // IMapTo<TDest, TSource>
                    foreach (var item in iMapToTypes)
                    {
                        var interfaces = item.GetInterfaces(false);
                        var interfaceTypes = interfaces.Where(r =>
                            r.IsGenericType && r.GenericTypeArguments.Length == 2 &&
                            r.FullName!.StartsWith(typeof(IMapTo<,>).FullName!)).ToList();
                        if (interfaceTypes.IsNullOrEmpty())
                            continue;
                        foreach (var interfaceType in interfaceTypes)
                        {
                            var destType = interfaceType.GenericTypeArguments[0];
                            var fromType = interfaceType.GenericTypeArguments[1];

                            var expression = method.MakeGenericMethod(fromType, destType)
                                .Invoke(config, new object[] {MemberList.None});
                            var expressionType = typeof(IMappingExpression<,>).MakeGenericType(fromType, destType);
                            var configMethod = item.GetMethods().First(r =>
                                r.Name == "Config" && r.GetParameters().Length == 1 &&
                                r.GetParameters().First().ParameterType == expressionType);
                            object obj;
                            try
                            {
                                obj = Activator.CreateInstance(item);
                            }
                            catch
                            {
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] can not create instance of {item}, must contains non-argument constructor.");
                            }

                            if (obj is null)
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] can not create instance of {item}, must contains non-argument constructor.");
                            try
                            {
                                configMethod.Invoke(obj, new object[] {expression});
                                using (obj as IDisposable)
                                {
                                }
                            }
                            catch (Exception e)
                            {
                                throw new InvalidConstraintException(
                                    $"[AutoMapper] create mapper {fromType}->{destType} error on type {item}.", e);
                            }
                        }
                    }
                }
            });

            configuration.AssertConfigurationIsValid();
            var mapper = new global::AutoMapper.Mapper(configuration);
            ShashlikAutoMapper.Instance = mapper;

            kernelService.Services.AddSingleton<IMapper>(mapper);
            return kernelService;
        }

        /// <summary>
        /// 查询映射
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IQueryable<TDest> QueryTo<TDest>(this IQueryable source)
        {
            if (ShashlikAutoMapper.Instance is null)
                throw new InvalidOperationException("shashlik mapper hasn't been initialized.");
            return source.ProjectTo<TDest>(ShashlikAutoMapper.Instance.ConfigurationProvider);
        }

        /// <summary>
        /// 对象映射 obj-><typeparamref name="TDest"/>
        /// </summary>
        /// <typeparam name="TDest">dest type</typeparam>
        /// <returns></returns>
        public static TDest MapTo<TDest>(this object obj)
        {
            if (ShashlikAutoMapper.Instance != null) return ShashlikAutoMapper.Instance.Map<TDest>(obj);
            throw new NullReferenceException($"Make sure AutoMapper initialized.");
        }

        /// <summary>
        /// 对象映射到已经存在的对象, obj-><typeparamref name="TDest"/>
        /// </summary>
        /// <param name="obj">source object</param>
        /// <param name="destObj">dest exists object</param>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDest">dest type</typeparam>
        public static void MapTo<TSource, TDest>(this TSource obj, TDest destObj)
        {
            if (ShashlikAutoMapper.Instance != null) ShashlikAutoMapper.Instance.Map(obj, destObj);
            else throw new NullReferenceException($"Make sure AutoMapper initialized.");
        }
    }
}