using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shashlik.Kernel;
using System;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.EfCore.Filter;

namespace Shashlik.EfCore
{
    /// <summary>
    /// Shashlik EfCore数据库上下文基类<para></para>
    /// 1. 完成自动注册实现<see cref="IEntity"/>接口实体<para></para>
    /// 2. 自动装载FluentApi配置,配置类可以使用依赖注入<para></para>
    /// 3. 已注册<see cref="ISoftDeleted"/>软删除全局过滤器, 实现<see cref="IEfCoreGlobalFilterRegister"/>接口并添加到服务容器完成全局过滤器的注册<para></para>
    /// 4. 默认已全局注册约定配置,string类型字段默认字符长度255<para></para>
    /// 5. DbContext配置时使用的服务(如过滤器注册/FluentApi配置类)上下文使用<see cref="GlobalKernelServiceProvider"/>,此为空则使用<see cref="IKernelServices"/>自行构建服务上下文
    /// </summary>
    public abstract class ShashlikDbContext<TDbContext> : DbContext
        where TDbContext : ShashlikDbContext<TDbContext>
    {

        /// <summary>
        /// 泛型必须是自己,约素构造函数的使用
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentException"></exception>
        protected ShashlikDbContext(DbContextOptions<TDbContext> options) : base(options)
        {
            if (typeof(TDbContext) != this.GetType())
                throw new ArgumentException($"Generic type {typeof(TDbContext)} must be {GetType()}");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
            // 添加默认的全局string类型约定,
            configurationBuilder.Properties<string>().HaveMaxLength(255);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var kernelServices = this.GetService<IKernelServices>();
            if (GlobalKernelServiceProvider.KernelServiceProvider is not null)
                modelBuilder.RegisterEntities<IEntity>(GlobalKernelServiceProvider.KernelServiceProvider);
            else
            {
                using var serviceProvider = kernelServices.Services.BuildServiceProvider();
                modelBuilder.RegisterEntities<IEntity>(serviceProvider);
            }
        }
    }
}