using Shashlik.Kernel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Redis
{
    public static class Extensions
    {
        /// <summary>
        /// add CRedisCore
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IKernelBuilder AddRedis(this IKernelBuilder kernelBuilder, IConfigurationSection configuration)
        {
            var options = configuration.Get<RedisOptions>();
            kernelBuilder.Services.Configure<RedisOptions>(configuration);
            var csRedis = new CSRedis.CSRedisClient(options.ConnectionString, options.Sentinels, options.Readonly);
            RedisHelper.Initialization(csRedis);
            kernelBuilder.Services.AddSingleton(csRedis);
            kernelBuilder.Services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
            return kernelBuilder;
        }
    }
}
