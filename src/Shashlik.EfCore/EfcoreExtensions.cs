using Microsoft.EntityFrameworkCore;
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

namespace Shashlik.EfCore
{
    public static class EfcoreExtensions
    {
        /// <summary>
        /// 增加EF配置映射,使用默认依赖上下文
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="dependencyContext"></param>
        /// <returns></returns>
        public static IKernelService AddEfEntityMappings(this IKernelService kernelBuilder, DependencyContext dependencyContext = null)
        {
            var assemblies = AssemblyHelper.GetReferredAssemblies(typeof(IEntityTypeConfiguration<>).Assembly, dependencyContext);
            return AddEfEntityMappings(kernelBuilder, assemblies);
        }

        /// <summary>
        /// 增加ef的配置映射
        /// </summary>
        /// <param name="kernelBuilder"></param>
        /// <param name="assemblies">指定程序集</param>
        /// <returns></returns>
        public static IKernelService AddEfEntityMappings(this IKernelService kernelBuilder, IEnumerable<Assembly> assemblies)
        {
            assemblies.ForEachItems(ass =>
            {
                kernelBuilder.Services.AddServiceByBasedOn(typeof(IEntityTypeConfiguration<>).GetTypeInfo(), ass, Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient);
            });
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
        /// <typeparam name="TEntityBase">实体基类</typeparam>
        /// <param name="assembly">程序集</param>
        /// <param name="registerAfter">注册后的操作,Type为实体类ix那个</param>
        public static void RegisterEntitiesFromAssembly<TEntityBase>(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Action<EntityTypeBuilder, Type> registerAfter = null,
            IServiceProvider serviceProvider = null)
            where TEntityBase : class
        {
            modelBuilder.RegisterEntitiesFromAssembly(assembly, r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r), registerAfter, serviceProvider);
        }

        /// <summary>
        /// 注册程序集中满足条件的所有类型
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">程序集</param>
        /// <param name="entityTypePredicate">实体注册条件</param>
        /// <param name="registerAfter">注册后的操作,Type为实体类ix那个</param>
        public static void RegisterEntitiesFromAssembly(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            Func<Type, bool> entityTypePredicate,
            Action<EntityTypeBuilder, Type> registerAfter = null,
            IServiceProvider serviceProvider = null)
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
                                 && t.IsChildTypeOfGenericType(typeof(IEntityTypeConfiguration<>))).ToList();

            HashSet<Type> registeredTypes = new HashSet<Type>();
            //存在fluent api配置的类,必须在Entity方法之前调用
            configTypes.ForEach(mappingType =>
            {
                var entityType = mappingType.GetTypeInfo().ImplementedInterfaces.First().GetGenericArguments().Single();

                //如果不满足条件的实体,不注册
                if (!entityTypePredicate(entityType))
                    return;

                var map = serviceProvider?.GetService(mappingType) ?? Activator.CreateInstance(mappingType);
                if (map == null)
                    throw new InvalidOperationException($"can not create instance of: {mappingType}!");
                applyConfigurationMethod.MakeGenericMethod(entityType)
                     .Invoke(modelBuilder, new object[] { map });

                registeredTypes.Add(entityType);
            });

            assembly
                .DefinedTypes
                .Where(entityTypePredicate)
                .ToList()
                .ForEach(r =>
                {
                    //直接调用Entity方法注册实体
                    var builder = modelBuilder.Entity(r);

                    if (r.IsChildTypeOfGenericType(typeof(ISoftDeleted<>)))
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

        static bool IsChildTypeOfGenericType(this Type childType, Type genericType)
        {
            var interfaceTypes = childType.GetTypeInfo().ImplementedInterfaces;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (childType.IsGenericType && childType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = childType.BaseType;
            if (baseType == null) return false;

            return IsChildTypeOfGenericType(baseType, genericType);
        }

        /// <summary>
        /// 执行迁移
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Migration<T>(this IServiceCollection services) where T : DbContext
        {
            using var locker = RedisHelper.Lock("EFCORE_DB_MIGRATION_LOCKING", 60);
            using var provider = services.BuildServiceProvider();
            using (var scope = provider.CreateScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<T>())
                dbContext.Database.Migrate();
        }
    }
}
