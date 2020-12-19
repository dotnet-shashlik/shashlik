using System.Threading;
using System.Threading.Tasks;
using CSRedis;
using Shashlik.Kernel;

namespace Shashlik.Redis
{
    public class RedisSnowflakeIdCleanAutowire : IApplicationStopAutowire
    {
        public RedisSnowflakeIdCleanAutowire(IRedisSnowflakeId redisSnowflakeId)
        {
            RedisSnowflakeId = redisSnowflakeId;
        }

        private IRedisSnowflakeId RedisSnowflakeId { get; }

        public Task OnStop(CancellationToken cancellationToken)
        {
            RedisSnowflakeId.Clean();
            return Task.CompletedTask;
        }
    }
}