using System;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shashlik.Utils.Extensions;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Shashlik.EfCore.Transactional
{
    public static class EfCoreTransactionalExtensions
    {
        public static IHostBuilder UseEfCoreTransactional<TDefaultDbContext>(this IHostBuilder builder)
            where TDefaultDbContext : DbContext
        {
            return builder.UseEfCoreTransactional(typeof(TDefaultDbContext));
        }

        public static IHostBuilder UseEfCoreTransactional(this IHostBuilder builder, Type defaultDbContextType)
        {
            if (!defaultDbContextType.IsSubTypeOf<DbContext>())
                throw new ArgumentException("Default dbContext type must inherit from DbContext.",
                    nameof(defaultDbContextType));
            TransactionalAttribute.DefaultDbContextType = defaultDbContextType;
            return builder.UseEfCoreTransactional();
        }

        public static IHostBuilder UseEfCoreTransactional(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
        }
    }
}