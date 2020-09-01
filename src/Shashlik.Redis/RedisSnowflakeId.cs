using Org.BouncyCastle.Crypto;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    /// <summary>
    /// 通过redis自动分配workid/dcid
    /// </summary>
    public class RedisSnowflakeId
    {
        static RedisSnowflakeId()
        {
            if (RedisHelper.Instance == null)
                throw new System.Exception($"redis not initialized.");
            var ids = IdGetter.Instance.GetIdFromRedis();
            idWorker = new SnowflakeId(ids.workId, ids.dcId);
        }

        public static SnowflakeId idWorker { get; }

        /// <summary>
        /// 雪花算法获取id,通过redis自动分配workid/dcid,适用于集群环境
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            return idWorker.NextId();
        }
    }
}
