using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Helpers;

namespace Shashlik.EfCore.Migration
{
    /// <summary>
    /// DbContext自动迁移装配, 自动注册[AutoMigration]
    /// </summary>
    [Transient]
    public class AutoMigrationAutowire : IServiceAssembler
    {
        private bool GetEnableAutoMigration(Type type, IConfiguration configuration)
        {
            var autoMigrationAttribute = type.GetCustomAttribute<AutoMigrationAttribute>();
            return autoMigrationAttribute is not null && autoMigrationAttribute.GetEnableAutoMigration(configuration);
        }

        public void Configure(IKernelServices kernelServices)
        {
            var dbContexts = ReflectionHelper.GetFinalSubTypes<DbContext>();
            foreach (var dbContext in dbContexts)
            {
                if (GetEnableAutoMigration(dbContext, kernelServices.RootConfiguration))
                    kernelServices.Services.AddAutoMigration(dbContext);
            }
        }
    }
}