using System;
using System.Threading;

// ReSharper disable InconsistentNaming

namespace Shashlik.Utils.Helpers
{
    /// <summary>
    /// 雪花算法ID生成器,参考美团Leaf,解决时钟回拨问题
    /// </summary>
    public class SnowflakeId
    {
        private readonly long twepoch;

        private const int workerIdBits = 10;

        //最大能够分配的workerid =1023
        private const int maxWorkerId = ~(-1 << workerIdBits);
        private const int sequenceBits = 12;
        private const int workerIdShift = sequenceBits;
        private const int timestampLeftShift = sequenceBits + workerIdBits;
        private const int sequenceMask = ~(-1 << sequenceBits);
        private readonly long workerId;
        private long sequence = 0L;
        private long lastTimestamp = -1L;


        /// <summary>
        /// 起始的时间戳
        /// </summary>
        /// <param name="workerId">workId,0~1023</param>
        /// <param name="twepoch">起始的时间戳,毫秒</param>
        public SnowflakeId(int workerId, long twepoch)
        {
            if (workerId is < 0 or > maxWorkerId)
                throw new ArgumentException("workerID must gte 0 and lte 1023");
            if (timeGen() <= twepoch)
                throw new ArgumentException("Snowflake not support twepoch gt currentTime");
            this.twepoch = twepoch;
            this.workerId = workerId;
        }

        public long NextId()
        {
            lock (this)
            {
                long timestamp = timeGen();
                if (timestamp < lastTimestamp)
                {
                    long offset = lastTimestamp - timestamp;
                    if (offset <= 5)
                    {
                        try
                        {
                            Monitor.Wait(this, (int)offset << 1);
                            timestamp = timeGen();
                            if (timestamp < lastTimestamp)
                            {
                                throw new Exception("clock back and wait error");
                            }
                        }
                        catch (ThreadInterruptedException e)
                        {
                            throw new Exception("thread interrupted", e);
                        }
                    }
                    else
                    {
                        throw new Exception("clock back gte 5ms");
                    }
                }

                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        //seq 为0的时候表示是下一毫秒时间开始对seq做随机
                        sequence = RandomHelper.Next(0, 100);
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    //如果是新的ms开始
                    sequence = RandomHelper.Next(0, 100);
                }

                lastTimestamp = timestamp;
                long id = ((timestamp - twepoch) << timestampLeftShift) | (workerId << workerIdShift) | sequence;
                return id;
            }
        }

        protected long tilNextMillis(long last)
        {
            long timestamp = timeGen();
            while (timestamp <= last)
            {
                timestamp = timeGen();
            }

            return timestamp;
        }

        protected long timeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public long getWorkerId()
        {
            return workerId;
        }
    }
}