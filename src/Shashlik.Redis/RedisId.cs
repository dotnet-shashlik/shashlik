using Shashlik.Utils.Common.SnowFlake;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Redis
{
    public class RedisId
    {
        private static IdWorker idWorker = new IdWorker(IdWorkerGetter.Instance.GetWorkerId(), 1);

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
