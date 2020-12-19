using System;
using CSRedis;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    public class DefaultRedisSnowflakeId : IRedisSnowflakeId
    {
        private const string AutoIncrementCacheKey = "REDIS_SNOWFLAKE_ID_AUTO_INCREMENMT";
        private const string ValueCacheKey = "REDIS_SNOWFLAKE_ID_VALUE_{0}";
        public static readonly string CurrentValue = Guid.NewGuid().ToString("n");

        public DefaultRedisSnowflakeId(CSRedisClient redisClient)
        {
            RedisClient = redisClient;
        }

        private CSRedisClient RedisClient { get; }

        public (int workId, int dcId) GetId()
        {
            for (int i = 0; i < 1024; i++)
            {
                var id = (int) RedisClient.IncrBy(AutoIncrementCacheKey);
                if (id > (SnowflakeId.MaxWorkerId + 1) * (SnowflakeId.MaxDatacenterId + 1))
                {
                    // 大于1024后重置为0
                    RedisClient.Set(AutoIncrementCacheKey, 0);
                    id = 1;
                }

                id -= 1;

                var valueKey = ValueCacheKey.Format(id);
                var value = RedisClient.Get(valueKey);
                // 占坑
                if (value.IsNullOrWhiteSpace() || value == CurrentValue)
                {
                    // 设置值，2小时有效期，每小时刷新1次
                    RedisHelper.Set(valueKey, CurrentValue, TimeSpan.FromHours(2));
                    TimerHelper.SetInterval(() => RedisHelper.Set(valueKey, CurrentValue, TimeSpan.FromHours(2)), TimeSpan.FromHours(1));
                    return (id >> 5, id % 32);
                }
            }

            throw new InvalidOperationException("Can't get workId and dataCenterId from redis.");
        }
    }
}