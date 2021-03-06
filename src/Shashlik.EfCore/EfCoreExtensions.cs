﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shashlik.Utils.Extensions;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shashlik.Utils.Helpers;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.EfCore
{
    public static class EfCoreExtensions
    {
        /// <summary>
        /// 0: DbContext Type
        /// </summary>
        public const string MigrationLockKey = "EFCORE_MIGRATION";

        /// <summary>
        /// 获取上下文所有的已注册的实体类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IReadOnlyList<Type> GetAllEntityTypes(this DbContext context)
        {
            if (context is null)
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
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        /// <param name="dependencyContext"></param>
        public static void RegisterEntities<TEntityBase>(
            this ModelBuilder modelBuilder,
            IServiceProvider? entityTypeConfigurationServiceProvider = null,
            DependencyContext? dependencyContext = null)
            where TEntityBase : class
        {
            var assemblies = ReflectionHelper.GetReferredAssemblies<TEntityBase>(dependencyContext);

            foreach (var item in assemblies)
                modelBuilder.RegisterEntitiesFromAssembly(item,
                    r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r),
                    entityTypeConfigurationServiceProvider);
        }

        /// <summary>
        /// 注册程序集中继承自<typeparamref name="TEntityBase"/>的所有类型
        /// </summary>
        /// <typeparam name="TEntityBase">实体基类</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">程序集</param>
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        public static void RegisterEntitiesFromAssembly<TEntityBase>(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            IServiceProvider? entityTypeConfigurationServiceProvider = null)
            where TEntityBase : class
        {
            modelBuilder.RegisterEntitiesFromAssembly(assembly,
                r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r),
                entityTypeConfigurationServiceProvider);
        }

        /// <summary>
        /// 注册程序集中满足条件的所有类型
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">扫描程序集</param>
        /// <param name="entityTypeFilter">实体类型过滤方法</param>
        /// <param name="entityTypeConfigurationServiceProvider">实体映射配置服务提供类</param>
        public static void RegisterEntitiesFromAssembly(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Func<Type, bool> entityTypeFilter,
            IServiceProvider? entityTypeConfigurationServiceProvider = null)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            //反射得到ModelBuilder的ApplyConfiguration<TEntity>(...)方法
            var applyConfigurationMethod = modelBuilder.GetType().GetMethods()
                .Single(r =>
                    {
                        if (r.Name != nameof(modelBuilder.ApplyConfiguration))
                            return false;
                        var ps = r.GetParameters();
                        return ps.Length == 1
                               && ps[0].ParameterType.IsGenericType
                               && ps[0].ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>);
                    }
                );

            //所有fluent api配置类
            var configTypes = assembly
                .DefinedTypes
                .Where(t =>
                    !t.IsAbstract && t.BaseType != null && t.IsClass
                    && t.IsSubTypeOfGenericType(typeof(IEntityTypeConfiguration<>))).ToList();

            var registeredTypes = new HashSet<Type>();
            //存在fluent api配置的类,必须在Entity方法之前调用
            configTypes.ForEach(mappingType =>
            {
                var entityType = mappingType.GetTypeInfo().ImplementedInterfaces.First().GetGenericArguments().Single();

                //如果不满足条件的实体,不注册
                if (!entityTypeFilter(entityType))
                    return;

                var map = entityTypeConfigurationServiceProvider?.GetService(mappingType) ??
                          Activator.CreateInstance(mappingType);
                if (map is null)
                    throw new InvalidOperationException($"can not create instance of: {mappingType}!");
                applyConfigurationMethod.MakeGenericMethod(entityType)
                    .Invoke(modelBuilder, new[] {map});

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
                });
        }

        /// <summary>
        /// 增加自动迁移类型
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void AddAutoMigration<TDbContext>(this IServiceCollection serviceCollection)
            where TDbContext : DbContext
        {
            serviceCollection.AddTransient<IAutoMigration, AutoMigration<TDbContext>>();
        }

        /// <summary>
        /// 增加自动迁移类型
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="dbContextType">上下文类型</param>
        public static void AddAutoMigration(this IServiceCollection serviceCollection, Type dbContextType)
        {
            if (dbContextType.IsClass
                && !dbContextType.IsAbstract
                && dbContextType.IsSubType<DbContext>())
            {
                serviceCollection.AddTransient(typeof(IAutoMigration),
                    typeof(AutoMigration<>).MakeGenericType(dbContextType));
            }
            else
                throw new ArgumentException(nameof(dbContextType));
        }

        /// <summary>
        /// 无锁执行迁移,服务注册阶段执行
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task MigrationWithoutLock<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await dbContext!.Database.MigrateAsync();
        }

        /// <summary>
        /// 无锁执行迁移,服务注册阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task MigrationWithoutLock(this IServiceCollection services, Type dbContextType)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            await dbContext!.Database.MigrateAsync();
        }

        /// <summary>
        /// 执行迁移,服务注册阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void Migration<TDbContext>(this IServiceCollection services, ILock? locker = null,
            int lockSecond = 60)
            where TDbContext : DbContext
        {
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();
            Migration<TDbContext>(scope.ServiceProvider, locker, lockSecond);
        }

        /// <summary>
        /// 执行迁移,服务注册阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextType"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        public static void Migration(this IServiceCollection services, Type dbContextType, ILock? locker = null,
            int lockSecond = 60)
        {
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();

            Migration(scope.ServiceProvider, dbContextType, locker, lockSecond);
        }


        /// <summary>
        /// 执行迁移,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void Migration<TDbContext>(this IServiceProvider provider, ILock? locker = null,
            int lockSecond = 60)
            where TDbContext : DbContext
        {
            locker ??= provider.GetRequiredService<ILock>();
            using var @lock = locker.Lock(MigrationLockKey.Format(typeof(TDbContext).FullName), lockSecond,
                waitTimeoutSeconds: lockSecond);
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            dbContext.Database.Migrate();
        }

        /// <summary>
        /// 执行迁移,应用配置阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="dbContextType"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Migration(this IServiceProvider provider, Type dbContextType, ILock? locker = null,
            int lockSecond = 60)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            locker ??= provider.GetRequiredService<ILock>();
            using var @lock = locker.Lock(MigrationLockKey, lockSecond,
                waitTimeoutSeconds: lockSecond);
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            dbContext!.Database.Migrate();
        }

        /// <summary>
        /// 执行迁移,服务注册阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task MigrationAsync<TDbContext>(this IServiceCollection services, ILock? locker = null,
            int lockSecond = 60)
            where TDbContext : DbContext
        {
            var rootServiceProvider = services.BuildServiceProvider();
            var scope = rootServiceProvider.CreateScope();

            await MigrationAsync<TDbContext>(scope.ServiceProvider, locker, lockSecond);
        }

        /// <summary>
        /// 执行迁移,服务注册阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextType"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <returns></returns>
        public static async Task MigrationAsync(this IServiceCollection services, Type dbContextType,
            ILock? locker = null, int lockSecond = 60)
        {
            var rootServiceProvider = services.BuildServiceProvider();
            var scope = rootServiceProvider.CreateScope();

            await MigrationAsync(scope.ServiceProvider, dbContextType, locker, lockSecond);
        }


        /// <summary>
        /// 执行迁移,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task MigrationAsync<TDbContext>(this IServiceProvider provider, ILock? locker = null,
            int lockSecond = 60)
            where TDbContext : DbContext
        {
            await MigrationAsync(provider, typeof(TDbContext), locker, lockSecond);
        }

        /// <summary>
        /// 执行迁移,应用配置阶段执行,locker为空则需要注册<see cref="ILock"/>服务
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="dbContextType">上下文类型</param>
        /// <param name="locker">自定义锁实例,null将从服务中获取</param>
        /// <param name="lockSecond">锁时长,一般根据迁移时长决定</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task MigrationAsync(this IServiceProvider provider, Type dbContextType,
            ILock? locker = null, int lockSecond = 60)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            locker ??= provider.GetRequiredService<ILock>();
            using var @lock = locker.Lock(MigrationLockKey, lockSecond,
                waitTimeoutSeconds: lockSecond);
            using var scope = provider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            await dbContext!.Database.MigrateAsync();
        }

        /// <summary>
        /// 执行自动迁移
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static IServiceProvider DoAutoMigration(this IServiceProvider serviceProvider)
        {
            var instances = serviceProvider.GetServices<IAutoMigration>();
            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("EfCoreAutoMigration");
            foreach (var item in instances)
            {
                var type = item.GetType();
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AutoMigration<>))
                {
                    var dbContextType = type.GetGenericArguments()[0];
                    try
                    {
                        serviceProvider.Migration(dbContextType);
                    }
                    catch (InvalidOperationException ex)
                    {
                        logger.LogError(ex, $"Migration DbContext of {dbContextType} occur error");
                    }
                }
            }

            return serviceProvider;
        }

        /// <summary>
        /// 执行自动迁移
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static IKernelServiceProvider DoAutoMigration(this IKernelServiceProvider serviceProvider)
        {
            DoAutoMigration((IServiceProvider) serviceProvider);
            return serviceProvider;
        }
    }
}