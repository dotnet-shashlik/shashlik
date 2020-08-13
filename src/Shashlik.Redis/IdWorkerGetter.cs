using CSRedis;
using Guc.Utils.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guc.Redis
{
    class IdWorkerGetter
    {

        private IdWorkerGetter()
        {

        }
        static IdWorkerGetter()
        {
            Instance = new IdWorkerGetter();
        }

        public static IdWorkerGetter Instance { get; }

        const string Prefix = "IDWORKER_GETTER:";
        const int expire = 60 * 60 + 5 * 60;

        private static int? id = null;

        public int GetWorkerId()
        {
            lock (this)
            {
                if (id.HasValue)
                    return id.Value;

                for (int i = 0; i < 32; i++)
                {
                    var key = Prefix + i;
                    if (RedisHelper.Set(key, i, expire, RedisExistence.Nx))
                    {
                        TimerHelper.SetTimeout(() =>
                        {
                            RedisHelper.Set(key, i, expire);

                        }, TimeSpan.FromHours(1));
                        id = i;
                        return i;
                    }
                }

                throw new Exception("未能从redis从计算得到workerId");
            }
        }
    }
}
