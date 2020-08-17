using Shashlik.Utils.Common;

namespace Shashlik.Redis
{
    /// <summary>
    /// 通过redis自动分配workid
    /// </summary>
    public class RedisSnowflakeId
    {
        private static SnowflakeIdHelper idWorker = new SnowflakeIdHelper(IdGetter.Instance.GetEffectiveId(), 1);

        /// <summary>
        /// 雪花算法获取id,通过redis自动计算WorkerId,适用于集群环境
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            return idWorker.NextId();
        }
    }
}
