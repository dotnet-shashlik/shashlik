﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shashlik.Kernel;
using System;

namespace Shashlik.EfCore
{
    /// <summary>
    /// Shashlik ef 上下文基类,完成自动注册实体,以及实体fluentApi配置
    /// </summary>
    public abstract class ShashlikDbContext : DbContext
    {
        /// <summary>
        /// 实体注册完成以后的钩子方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="entityType">实体类型</param>
        protected virtual void EntityRegisterAfter(EntityTypeBuilder builder, Type entityType) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.RegisterEntities<IEntity>(
                entityTypeConfigurationServiceProvider: this.GetService<IServiceProvider>(),
                registerAfter: EntityRegisterAfter,
                dependencyContext: this.GetService<IKernelServices>().ScanFromDependencyContext);
        }
    }
}
