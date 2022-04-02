using Microsoft.EntityFrameworkCore;
using System;
using Shashlik.Utils.Extensions;
using System.Threading.Tasks;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.EfCore.Migration
{
    public static class EfCoreExtensions
    {
        /// <summary>
        /// 无锁执行迁移,服务注册阶段执行,适用单体应用
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
            await dbContext.Database.MigrateAsync();
        }

        /// <summary>
        /// 无锁执行迁移,服务注册阶段执行,适用单体应用
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
        /// 无锁执行迁移,程序运行阶段执行,适用单体应用
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task MigrationWithoutLock<TDbContext>(this IServiceProvider serviceProvider)
            where TDbContext : DbContext
        {
            using var scope = serviceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        /// <summary>
        /// 无锁执行迁移,程序运行阶段执行,适用单体应用
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="dbContextType"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task MigrationWithoutLock(this IServiceProvider serviceProvider, Type dbContextType)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            using var scope = serviceProvider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            await dbContext!.Database.MigrateAsync();
        }

        /// <summary>
        /// 有锁执行迁移,服务注册阶段执行
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void Migration<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();
            Migration<TDbContext>(scope.ServiceProvider);
        }

        /// <summary>
        /// 有锁执行迁移,服务注册阶段执行
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextType"></param>
        public static void Migration(this IServiceCollection services, Type dbContextType)
        {
            var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();

            Migration(scope.ServiceProvider, dbContextType);
        }

        /// <summary>
        /// 有锁程序运行阶段执行迁移
        /// </summary>
        /// <param name="provider"></param>
        /// <typeparam name="TDbContext"></typeparam>
        public static void Migration<TDbContext>(this IServiceProvider provider)
            where TDbContext : DbContext
        {
            var locker = provider.GetRequiredService<IEfMigrationLock>();
            using var @lock = locker.Lock();
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            dbContext.Database.Migrate();
        }

        /// <summary>
        /// 有锁程序运行阶段执行迁移
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="dbContextType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void Migration(this IServiceProvider provider, Type dbContextType)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            var locker = provider.GetRequiredService<IEfMigrationLock>();
            using var @lock = locker.Lock();
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            dbContext!.Database.Migrate();
        }

        /// <summary>
        /// 有锁服务注册阶段执行迁移
        /// </summary>
        /// <param name="services"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static async Task MigrationAsync<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            await MigrationAsync(services, typeof(TDbContext));
        }

        /// <summary>
        /// 有锁服务注册阶段执行迁移
        /// </summary>
        /// <param name="services"></param>
        /// <param name="dbContextType"></param>
        /// <returns></returns>
        public static async Task MigrationAsync(this IServiceCollection services, Type dbContextType)
        {
            await using var rootServiceProvider = services.BuildServiceProvider();
            using var scope = rootServiceProvider.CreateScope();
            await MigrationAsync(scope.ServiceProvider, dbContextType);
        }

        /// <summary>
        /// 有锁程序运行阶段执行迁移
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static async Task MigrationAsync<TDbContext>(this IServiceProvider provider)
            where TDbContext : DbContext
        {
            await MigrationAsync(provider, typeof(TDbContext));
        }

        /// <summary>
        /// 有锁程序运行阶段执行迁移
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="dbContextType">上下文类型</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task MigrationAsync(this IServiceProvider provider, Type dbContextType)
        {
            if (!dbContextType.IsSubTypeOrEqualsOf<DbContext>())
                throw new InvalidOperationException($"Auto migration type error: {dbContextType}");
            var locker = provider.GetRequiredService<IEfMigrationLock>();
            using var @lock = locker.Lock();
            using var scope = provider.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
            await dbContext!.Database.MigrateAsync();
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