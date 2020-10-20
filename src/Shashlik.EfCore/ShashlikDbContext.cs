﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.Kernel;
using System;
using System.Data;

namespace Shashlik.EfCore
{
    /// <summary>
    /// Shashlik ef 上下文基类,完成自动注册实体,以及自动装载fluentApi配置
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
                throw new ArgumentException($"Generic type {typeof(TDbContext)} must be {GetType()}.");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RegisterEntities<IEntity>(
                entityTypeConfigurationServiceProvider: this.GetService<IServiceProvider>(),
                dependencyContext: this.GetService<IKernelServices>().ScanFromDependencyContext);
        }
    }
}