using CSRedis;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    public class DefaultRedisSnowflakeId : IRedisSnowflakeId
    {
        private const string CacheKey = "REDIS_SNOWFLAKE_ID_GETTER";

        public DefaultRedisSnowflakeId(CSRedisClient redisClient)
        {
            RedisClient = redisClient;
        }

        private CSRedisClient RedisClient { get; }

        public (int workId, int dcId) GetId()
        {
            var id = (int) RedisClient.IncrBy(CacheKey);
            if (id > (SnowflakeId.MaxWorkerId + 1) * (SnowflakeId.MaxDatacenterId + 1))
            {
                // 大于1024后重置为0
                RedisClient.Set(CacheKey, 0);
                id = 1;
            }

            id -= 1;
            return (id >> 5, id % 32);
        }
    }
}