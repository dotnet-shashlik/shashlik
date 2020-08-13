using Guc.Kernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TencentCos.Sdk
{
    public static class Extensions
    {
        /// <summary>
        /// 增加腾讯云cos服务,配置项为单例
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IKernelBuilder AddTencentCos(this IKernelBuilder builder, IConfigurationSection configuration)
        {
            builder.Services.Configure<TencentCosOptions>(configuration);
            builder.Services.AddTransient<ITencentCos, DefaultTencentCos>();
            return builder;
        }
    }
}
