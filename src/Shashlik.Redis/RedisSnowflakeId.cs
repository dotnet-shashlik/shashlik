using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    /// <summary>
    /// 通过redis自动分配WorkId/DcId
    /// </summary>
    public class RedisSnowflakeId
    {
        static RedisSnowflakeId()
        {
            if (RedisHelper.Instance == null)
                throw new System.Exception($"redis not initialized.");
            var (workId, dcId) = RedisSnowflakeIdCalculator.GetIdFromRedis();

            WorkerId = workId;
            DatacenterId = dcId;
            IdWorker = new SnowflakeId(workId, dcId);
        }

        public static SnowflakeId IdWorker { get; }

        public static long? WorkerId { get; }
        public static long? DatacenterId { get; }

        /// <summary>
        /// 雪花算法获取id,通过redis自动分配WorkId/DcId,适用于集群环境
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            return IdWorker.NextId();
        }
    }
}