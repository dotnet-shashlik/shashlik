using System;
using System.Linq;
using FreeRedis;
using Shashlik.Kernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shashlik.Kernel.Attributes;
using Shashlik.Kernel.Dependency;
using Shashlik.Utils.Extensions;

namespace Shashlik.Redis
{
    /// <summary>
    /// redis自动装配,装配顺序200
    /// </summary>
    [Order(200)]
    [Transient]
    public class RedisAssembler : IServiceAssembler
    {
        public RedisAssembler(IOptions<RedisOptions> options)
        {
            Options = options.Value;
        }

        private RedisOptions Options { get; }

        public void Configure(IKernelServices kernelService)
        {
            if (!Options.Enable)
                return;

            RedisClient redisClient;
            switch (Options.Mode)
            {
                case RedisOptions.RedisMode.Default:
                {
                    if (string.IsNullOrWhiteSpace(Options.ConnectionString))
                    {
                        throw new OptionsValidationException("Shashlik.Redis.ConnectionString", typeof(RedisOptions),
                            new[] { "ConnectionString can not be empty." });
                    }


                    redisClient =
                        new RedisClient(Options.ConnectionString,
                            Options.SlaveConnectionStrings?.Select(ConnectionStringBuilder.Parse).ToArray() ??
                            Array.Empty<ConnectionStringBuilder>());
                    break;
                }
                case RedisOptions.RedisMode.Sentinel:
                {
                    if (string.IsNullOrWhiteSpace(Options.ConnectionString))
                    {
                        throw new OptionsValidationException("Shashlik.Redis.ConnectionString", typeof(RedisOptions),
                            new[] { "ConnectionString can not be empty." });
                    }

                    if (Options.Sentinels.IsNullOrEmpty())
                    {
                        throw new OptionsValidationException("Shashlik.Redis.Sentinels", typeof(RedisOptions),
                            new[] { "Sentinels can not be empty." });
                    }

                    redisClient =
                        new RedisClient(Options.ConnectionString, Options.Sentinels, Options.RwSplitting);
                    break;
                }
                case RedisOptions.RedisMode.Cluster:
                {
                    if (Options.ClusterConnectionStrings.IsNullOrEmpty())
                    {
                        throw new OptionsValidationException("Shashlik.Redis.ClusterConnectionStrings",
                            typeof(RedisOptions),
                            new[] { "ClusterConnectionStrings can not be empty." });
                    }

                    redisClient =
                        new RedisClient(Options.ClusterConnectionStrings!.Select(ConnectionStringBuilder.Parse)
                            .ToArray());
                    break;
                }
                default: throw new IndexOutOfRangeException();
            }

            // 配置默认使用Newtonsoft.Json作为序列化工具
            redisClient.Serialize = JsonConvert.SerializeObject;
            redisClient.Deserialize = JsonConvert.DeserializeObject;

            kernelService.Services.AddSingleton(redisClient);
        }
    }
}