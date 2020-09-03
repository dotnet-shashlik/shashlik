using CSRedis;
using System;
using System.Threading;
using Shashlik.Utils.Extensions;
using Shashlik.Utils.Helpers;

namespace Shashlik.Redis
{
    /// <summary>
    /// 雪花算法 WorkId/DcId自动计算器
    /// </summary>
    internal static class RedisSnowflakeIdCalculator
    {
        static RedisSnowflakeIdCalculator()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => Source.Cancel();
        }

        private const string WorkerIdPrefix = "SNOWFLAKE_WORKERID_GETTER:{0}";
        private const string DcIdPrefix = "SNOWFLAKE_DCID_GETTER:{0}";
        private const int Expire = 60 * 60 + 5 * 60;
        private static int? _workId = null;
        private static int? _dcId = null;
        private static readonly object Locker = new object();
        // 程序退出时取消任务
        private static readonly CancellationTokenSource Source = new CancellationTokenSource();

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
                        var key = WorkerIdPrefix.Format(i);
                        if (RedisHelper.Set(key, i, Expire, RedisExistence.Nx))
                        {
                            // 每小时刷新下数据
                            TimerHelper.SetInterval(() => { RedisHelper.Set(key, i, Expire); }, TimeSpan.FromHours(1),
                                Source.Token);
                            _workId = i;
                        }
                    }

                    throw new Exception("未能从redis从计算得到workerId");
                }

                if (_dcId.HasValue) return (_workId.Value, _dcId.Value);
                {
                    for (var i = 0; i < 32; i++)
                    {
                        var key = DcIdPrefix.Format(i);
                        if (!RedisHelper.Set(key, i, Expire, RedisExistence.Nx)) continue;
                        // 每小时刷新下数据
                        TimerHelper.SetInterval(() => { RedisHelper.Set(key, i, Expire); }, TimeSpan.FromHours(1),
                            Source.Token);
                        _dcId = i;
                    }

                    throw new Exception("未能从redis从计算得到dcId");
                }
            }
        }
    }
}