using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Shashlik.Utils.Extensions;
using Microsoft.Extensions.DependencyModel;
using Shashlik.Utils.Helpers;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable InvertIf
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.EfCore
{
    public static class EfCoreExtensions
    {
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
        /// 注册程序集中继承自<typeparamref name="TEntityBase"/>的所有类型<para></para>
        /// 实体映射配置服务提供类,可以通过服务注入配置类型,不存在时,则反射创建
        /// </summary>
        /// <typeparam name="TEntityBase"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="serviceProvider">实体映射配置服务提供类,可以通过服务注入配置类型,不存在时,则反射创建</param>
        public static void RegisterEntities<TEntityBase>(
            this ModelBuilder modelBuilder,
            IServiceProvider serviceProvider)
            where TEntityBase : class
        {
            RegisterEntities<TEntityBase>(modelBuilder, serviceProvider,
                r => serviceProvider.GetService(r) ?? Activator.CreateInstance(r));
        }

        /// <summary>
        /// 注册整个运行时程序集中继承自<typeparamref name="TEntityBase"/>的所有类型
        /// </summary>
        /// <typeparam name="TEntityBase"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="fluentConfigClassInstanceSupplier">实体FluentApi配置类创建器</param>
        public static void RegisterEntities<TEntityBase>(
            this ModelBuilder modelBuilder,
            IServiceProvider serviceProvider,
            Func<Type, object?> fluentConfigClassInstanceSupplier)
            where TEntityBase : class
        {
            var assemblies = ReflectionHelper.GetReferredAssemblies<TEntityBase>(DependencyContext.Default);

            foreach (var item in assemblies)
                modelBuilder.RegisterEntitiesFromAssembly<TEntityBase>(item, serviceProvider,
                    fluentConfigClassInstanceSupplier);
        }


        /// <summary>
        /// 注册指定程序集中继承自<typeparamref name="TEntityBase"/>的所有类型
        /// </summary>
        /// <typeparam name="TEntityBase">实体基类</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">程序集</param>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="fluentConfigClassInstanceSupplier">实体FluentApi配置类创建器</param>
        public static void RegisterEntitiesFromAssembly<TEntityBase>(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            IServiceProvider serviceProvider,
            Func<Type, object?> fluentConfigClassInstanceSupplier)
            where TEntityBase : class
        {
            modelBuilder.RegisterEntitiesFromAssembly(
                assembly,
                serviceProvider,
                r => !r.IsAbstract && r.IsClass && typeof(TEntityBase).IsAssignableFrom(r),
                fluentConfigClassInstanceSupplier
            );
        }

        /// <summary>
        /// 注册程序集中满足条件的所有类型
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assembly">扫描程序集</param>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="entityTypeFilter">实体类型过滤方法</param>
        /// <param name="fluentConfigClassInstanceSupplier">实体FluentApi配置类创建器</param>
        public static void RegisterEntitiesFromAssembly(
            this ModelBuilder modelBuilder,
            Assembly assembly,
            IServiceProvider serviceProvider,
            Func<Type, bool> entityTypeFilter,
            Func<Type, object?> fluentConfigClassInstanceSupplier)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            // 反射得到ModelBuilder的ApplyConfiguration<TEntity>(...)方法
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

            // 所有fluent api配置类
            var configTypes = assembly
                .DefinedTypes
                .Where(t =>
                    !t.IsAbstract
                    && t.BaseType is not null
                    && t.IsClass
                    && t.IsSubTypeOfGenericType(typeof(IEntityTypeConfiguration<>)))
                .ToList();

            var registeredTypes = new HashSet<Type>();
            // 存在fluent api配置的类,必须在Entity方法之前调用
            configTypes.ForEach(mappingType =>
            {
                var entityType = mappingType.GetTypeInfo().ImplementedInterfaces.First().GetGenericArguments().Single();

                // 如果不满足条件的实体,不注册
                if (!entityTypeFilter(entityType))
                    return;

                var map = fluentConfigClassInstanceSupplier(mappingType);
                if (map is null)
                    throw new InvalidOperationException($"can not create instance of: {mappingType}!");
                applyConfigurationMethod.MakeGenericMethod(entityType).Invoke(modelBuilder, new[] { map });

                registeredTypes.Add(entityType);
            });

            var filters = serviceProvider.GetServices<IEfCoreGlobalFilterRegister>();
            var filterRegisterMap = new Dictionary<Type, IEfCoreGlobalFilterRegister>();
            foreach (var item in filters)
            {
                var type = item.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(r =>
                        r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IEfCoreGlobalFilterRegister<>));
                if (type == null)
                    continue;
                var filterType = type.GetGenericArguments()[0];
                filterRegisterMap[filterType] = item;
            }

            assembly
                .DefinedTypes
                .Where(entityTypeFilter)
                .ToList()
                .ForEach(entityType =>
                {
                    // 直接调用Entity方法注册实体
                    var builder = modelBuilder.Entity(entityType);

                    // 注册软删除过滤器
                    foreach (var keyValuePair in filterRegisterMap)
                    {
                        if (keyValuePair.Key.IsAssignableFrom(entityType))
                        {
                            var method = keyValuePair.Value.GetType()
                                .GetMethod(nameof(IEfCoreGlobalFilterRegister<ISoftDeleted>.HasQueryFilter),
                                    Type.EmptyTypes)
                                !.MakeGenericMethod(entityType);
                            var exp = (LambdaExpression?)method.Invoke(keyValuePair.Value, new object[] { });
                            builder.HasQueryFilter(exp);
                        }
                    }

                    // 注册[JsonField]
                    entityType.GetProperties()
                        .Where(r => r.CanRead && r.CanWrite && !r.GetIndexParameters().Any() &&
                                    r.IsDefinedAttribute<JsonFieldAttribute>(true))
                        .ForEachItem(property =>
                        {
                            builder.Property(property.PropertyType, property.Name)
                                .HasConversion(typeof(JsonValueConverter<>).MakeGenericType(property.PropertyType),
                                    typeof(JsonValueComparer<>).MakeGenericType(property.PropertyType));
                        });
                });
        }
    }
}