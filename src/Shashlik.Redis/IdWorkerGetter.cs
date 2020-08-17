using CSRedis;
using Shashlik.Utils.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shashlik.Redis
{
    class IdGetter
    {

        private IdGetter()
        {

        }
        static IdGetter()
        {
            Instance = new IdGetter();
        }

        public static IdGetter Instance { get; }

        const string WorkerIdPrefix = "SNOWFLAKE_WORKERID_GETTER:";
        const string DcIdPrefix = "SNOWFLAKEID_DCID_GETTER:";
        const int expire = 60 * 60 + 5 * 60;

        private static int? workId = null;

        private static int? DcId = null;

        /// <summary>
        /// 得到一个有效的id
        /// </summary>
        /// <returns></returns>
        public int GetEffectiveId()
        {
            lock (Instance)
            {
                if (workId.HasValue)
                    return workId.Value;
                else
                {
                    for (int i = 0; i < 32; i++)
                    {
                        var key = WorkerIdPrefix + i;
                        if (RedisHelper.Set(key, i, expire, RedisExistence.Nx))
                        {
                            TimerHelper.SetTimeout(() =>
                            {
                                RedisHelper.Set(key, i, expire);
                            }, TimeSpan.FromHours(1));
                            workId = i;
                            return i;
                        }
                    }

                    throw new Exception("未能从redis从计算得到workerId");
                }
            }
        }
    }
}
