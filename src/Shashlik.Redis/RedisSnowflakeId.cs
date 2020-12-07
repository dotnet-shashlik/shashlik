using System;
using Microsoft.Extensions.DependencyInjection;
using Shashlik.Kernel;
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
            if (RedisHelper.Instance is null)
                throw new InvalidOperationException($"Redis uninitialized");
            var redisSnowflakeId = GlobalKernelServiceProvider.KernelServiceProvider!.GetRequiredService<IRedisSnowflakeId>();
            var id = redisSnowflakeId.GetId();
            WorkerId = id.workId;
            DatacenterId = id.dcId;
            IdWorker = new SnowflakeId(WorkerId.Value, DatacenterId.Value);
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