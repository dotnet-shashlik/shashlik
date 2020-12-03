using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shashlik.Kernel;
using Shashlik.Kernel.Attributes;
using Shashlik.Utils.Helpers;

namespace Shashlik.EfCore
{
    /// <summary>
    /// DbContext自动迁移装配, 自动注册[AutoMigration]
    /// </summary>
    public class AutoMigrationAutowire : IServiceAutowire
    {
        public AutoMigrationAutowire(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        private bool GetEnableAutoMigration(Type type)
        {
            var autoMigrationAttribute = type.GetCustomAttribute<AutoMigrationAttribute>();
            return autoMigrationAttribute != null && autoMigrationAttribute.GetEnableAutoMigration(Configuration);
        }

        public void Configure(IKernelServices kernelServices)
        {
            var dbContexts = ReflectionHelper.GetFinalSubTypes<DbContext>();
            foreach (var dbContext in dbContexts)
            {
                if (GetEnableAutoMigration(dbContext))
                    kernelServices.Services.AddAutoMigration(dbContext);
            }
        }
    }
}