﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.Utils.Extensions;
using System.Linq.Expressions;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Utils.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Shashlik.EfCore
{
    public static class EfCoreExtensions
    {
        /// <summary>
        /// 增加EF配置映射,注册所有的IEntityTypeConfiguration<>实现类
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <returns></returns>
        public static IKernelServices AddEfEntityMappings(this IKernelServices kernelBuilder)
        {
            kernelBuilder.AddServiceByBasedOn(typeof(IEntityTypeConfiguration<>).GetTypeInfo(), ServiceLifetime.Transient);
            return kernelBuilder;
        }

        /// <summary>
        /// 获取上下文所有的已注册的实体类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetAllEntityTypes(this DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Model.GetEntityTypes().Select(r => r.ClrType).ToList();
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TEntityBase"/>的所有类型
        /// </summary>
        /// <typeparam name="TEntityBase"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="registerAfter"></param>
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        /// <param name="dependencyContext"></param>
        public static void RegisterEntities<TEntityBase>(
            this ModelBuilder modelBuilder,
            Action<EntityTypeBuilder, Type> registerAfter = null,
            IServiceProvider entityTypeConfigurationServiceProvider = null,
            DependencyContext dependencyContext = null)
            where TEntityBase : class
        {
            var assemblies = AssemblyHelper.GetReferredAssemblies<TEntityBase>(dependencyContext);

            foreach (var item in assemblies)
                modelBuilder.RegisterEntitiesFromAssembly(item, r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r), registerAfter, entityTypeConfigurationServiceProvider);
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TEntityBase"/>的所有类型
        /// </summary>
        /// <typeparam name="TEntityBase">实体基类</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">程序集</param>
        /// <param name="registerAfter">注册后的操作,Type为实体类</param>
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        public static void RegisterEntitiesFromAssembly<TEntityBase>(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Action<EntityTypeBuilder, Type> registerAfter = null,
            IServiceProvider entityTypeConfigurationServiceProvider = null)
            where TEntityBase : class
        {
            modelBuilder.RegisterEntitiesFromAssembly(assembly, r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r), registerAfter, entityTypeConfigurationServiceProvider);
        }

        /// <summary>
        /// 注册程序集中满足条件的所有类型
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">扫描程序集</param>
        /// <param name="entityTypeFilter">实体类型过滤方法</param>
        /// <param name="registerAfter">注册后的操作,Type为实体类</param>
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        public static void RegisterEntitiesFromAssembly(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Func<Type, bool> entityTypeFilter,
            Action<EntityTypeBuilder, Type> registerAfter = null,
            IServiceProvider entityTypeConfigurationServiceProvider = null)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            //反射得到ModelBuilder的ApplyConfiguration<TEntity>(...)方法
            var applyConfigurationMethod = modelBuilder.GetType().GetMethods().First(r =>
            {
                var genericArguments = r.GetGenericArguments();
                return r.Name == "ApplyConfiguration" && genericArguments.Length == 1 && genericArguments[0].Name == "TEntity";
            });

            //所有fluent api配置类
            var configTypes = assembly
                               .DefinedTypes
                               .Where(t =>
                                 !t.IsAbstract && t.BaseType != null && t.IsClass
                                 && t.IsSubTypeOfGenericType(typeof(IEntityTypeConfiguration<>))).ToList();

            HashSet<Type> registeredTypes = new HashSet<Type>();
            //存在fluent api配置的类,必须在Entity方法之前调用
            configTypes.ForEach(mappingType =>
            {
                var entityType = mappingType.GetTypeInfo().ImplementedInterfaces.First().GetGenericArguments().Single();

                //如果不满足条件的实体,不注册
                if (!entityTypeFilter(entityType))
                    return;

                var map = entityTypeConfigurationServiceProvider?.GetService(mappingType) ?? Activator.CreateInstance(mappingType);
                if (map == null)
                    throw new InvalidOperationException($"can not create instance of: {mappingType}!");
                applyConfigurationMethod.MakeGenericMethod(entityType)
                     .Invoke(modelBuilder, new object[] { map });

                registeredTypes.Add(entityType);
            });

            assembly
                .DefinedTypes
                .Where(entityTypeFilter)
                .ToList()
                .ForEach(r =>
                {
                    //直接调用Entity方法注册实体
                    var builder = modelBuilder.Entity(r);

                    if (r.IsSubTypeOfGenericType(typeof(ISoftDeleted<>)))
                    {
                        var item = Expression.Parameter(r, "r");
                        var prop = Expression.Property(item, "IsDeleted");
                        var value = Expression.Constant(false);
                        var equals = Expression.Equal(prop, value);
                        var lambda = Expression.Lambda(equals, item);

                        builder.HasQueryFilter(lambda);
                    }
                    registerAfter?.Invoke(builder, r);
                });
        }

        /// <summary>
        /// 增加数据库上下文开启事务的方式
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="kernelService"></param>
        /// <param name="beginTransactionFunc"></param>
        public static void AddBeginTransationFunction<TDbContext>(this IKernelServices kernelService, Func<TDbContext, IDbContextTransaction> beginTransactionFunc)
            where TDbContext : DbContext
        {
            kernelService.Services.AddSingleton(
                typeof(IEfNestedTransactionFunction<TDbContext>),
                new DefaultEfNestedTransactionFunction<TDbContext>(beginTransactionFunc));
        }
    }
}
