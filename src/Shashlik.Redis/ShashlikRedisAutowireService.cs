﻿using Shashlik.Kernel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel.Autowire;
using Microsoft.Extensions.Options;

namespace Shashlik.Redis
{
    public class ShashlikRedisAutowireService : IAutowireConfigureService
    {
        public ShashlikRedisAutowireService(IOptions<RedisOptions> options)
        {
            Options = options.Value;
        }

        RedisOptions Options { get; }

        public void ConfigureServices(IKernelService kernelService)
        {
            var csRedis = new CSRedis.CSRedisClient(Options.ConnectionString, Options.Sentinels, Options.Readonly);
            RedisHelper.Initialization(csRedis);
            kernelService.Services.AddSingleton(csRedis);
            kernelService.Services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
        }
    }
}
