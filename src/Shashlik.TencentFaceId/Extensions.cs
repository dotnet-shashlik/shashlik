using Shashlik.Kernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace TencentFaceId.Sdk
{
    public static class Extensions
    {
        /// <summary>
        /// 增加腾讯云faceid服务,配置项为单例
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IKernelBuilder AddTencentFaceId(this IKernelBuilder builder, IConfigurationSection configuration, bool useEmpty = false)
        {
            builder.Services.Configure<TencentFaceIdOptions>(configuration);
            if (useEmpty)
            {
                builder.Services.AddSingleton<ITencentFaceId, EmptyFaceId>();
            }
            else
            {
                builder.Services.AddSingleton<ITencentFaceId, DefaultTencentFaceId>();
            }

            return builder;
        }
    }
}
