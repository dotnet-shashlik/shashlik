using CSRedis;
using System;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    /// <summary>
    /// 雪花算法 WorkId/DcId自动计算器
    /// </summary>
    internal static class RedisSnowflakeIdCalculator
    {
        private const string WorkerIdPrefix = "SNOWFLAKE_WORKERID_GETTER:";
        private const string DcIdPrefix = "SNOWFLAKE_DCID_GETTER:";
        private const int Expire = 60 * 60 + 5 * 60;

        private static int? _workId = null;

        private static int? _dcId = null;
        private static readonly object Locker = new object();

        /// <summary>
        /// 得到一个有效的id
        /// </summary>
        /// <returns></returns>
        public static (int workId, int dcId) GetIdFromRedis()
        {
            lock (Locker)
            {
                if (!_workId.HasValue)
                {
                    for (var i = 0; i < 32; i++)
                    {
                        var key = WorkerIdPrefix + i;
                        if (RedisHelper.Set(key, i, Expire, RedisExistence.Nx))
                        {
                            // 每小时刷新下数据
                            TimerHelper.SetInterval(() => { RedisHelper.Set(key, i, Expire); }, TimeSpan.FromHours(1));
                            _workId = i;
                        }
                    }

                    throw new Exception("未能从redis从计算得到workerId");
                }

                if (_dcId.HasValue) return (_workId.Value, _dcId.Value);
                {
                    for (var i = 0; i < 32; i++)
                    {
                        var key = DcIdPrefix + i;
                        if (!RedisHelper.Set(key, i, Expire, RedisExistence.Nx)) continue;
                        // 每小时刷新下数据
                        TimerHelper.SetInterval(() => { RedisHelper.Set(key, i, Expire); }, TimeSpan.FromHours(1));
                        _dcId = i;
                    }

                    throw new Exception("未能从redis从计算得到dcId");
                }
            }
        }
    }
}