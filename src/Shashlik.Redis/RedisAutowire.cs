using CSRedis;
using Shashlik.Kernel;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Redis
{
    /// <summary>
    /// redis自动装配,装配顺序200
    /// </summary>
    [Order(200)]
    public class RedisAutowire : IServiceAutowire
    {
        public RedisAutowire(IOptions<RedisOptions> options)
        {
            Options = options.Value;
        }

        private RedisOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            lock (Options)
            {
                var csRedis = new CSRedisClient(Options.ConnectionString, Options.Sentinels, Options.Readonly);
                RedisHelper.Initialization(csRedis);
                kernelService.Services.AddSingleton(csRedis);
                kernelService.Services.AddSingleton<IDistributedCache>(
                    new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));
                if (Options.EnableRedisLock)
                    kernelService.Services.AddSingleton<ILock, RedisLock>();
            }
        }
    }
}