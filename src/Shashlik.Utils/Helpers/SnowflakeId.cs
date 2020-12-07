using System;

namespace Shashlik.Utils.Helpers
{
    public sealed class SnowflakeId
    {
        public const long Twepoch = 1288834974657L;
        public const int WorkerIdBits = 5;
        public const int DatacenterIdBits = 5;
        public const int SequenceBits = 12;
        public const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        public const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);
        public const int WorkerIdShift = SequenceBits;
        public const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        public const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private readonly object _lock = new object();
        private long _lastTimestamp = -1L;

        public SnowflakeId(long workerId, long datacenterId, long sequence = 0L)
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            Sequence = sequence;

            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0)
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
                throw new ArgumentException($"datacenter Id can't be greater than {MaxDatacenterId} or less than 0");
        }

        private long WorkerId { get; }
        private long DatacenterId { get; }
        private long Sequence { get; set; }

        public long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp)
                    throw new Exception(
                        $"InvalidSystemClock: Clock moved backwards, Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");

                if (_lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & SequenceMask;
                    if (Sequence == 0) timestamp = TilNextMillis(_lastTimestamp);
                }
                else
                {
                    Sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (DatacenterId << DatacenterIdShift) |
                         (WorkerId << WorkerIdShift) | Sequence;

                return id;
            }
        }

        private long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) timestamp = TimeGen();
            return timestamp;
        }

        private long TimeGen()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}