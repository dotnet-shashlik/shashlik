// copy from: https://github.com/2881099/csredis/blob/master/src/CSRedisCore/CSRedisClient.cs
// copyright is CSRedisCore https://github.com/2881099/csredis/blob/master/LICENSE

using System;
using System.Threading;
using CSRedis;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable StringLiteralTypo

namespace Shashlik.Redis
{
    public class CSRedisClientLock : IDisposable
    {
        CSRedisClient _client;
        string _name;
        string _value;
        int _timeoutSeconds;
        Timer? _autoDelayTimer;

        internal CSRedisClientLock(CSRedisClient rds, string name, string value, int timeoutSeconds, bool autoDelay)
        {
            _client = rds;
            _name = name;
            _value = value;
            _timeoutSeconds = timeoutSeconds;
            if (autoDelay)
            {
                var milliseconds = _timeoutSeconds * 1000 / 2;
                _autoDelayTimer = new Timer(state2 => Delay(milliseconds), null, milliseconds, milliseconds);
            }
        }

        /// <summary>
        /// 延长锁时间，锁在占用期内操作时返回true，若因锁超时被其他使用者占用则返回false
        /// </summary>
        /// <param name="milliseconds">延长的毫秒数</param>
        /// <returns>成功/失败</returns>
        public bool Delay(int milliseconds)
        {
            var ret = _client.Eval(@"local gva = redis.call('GET', KEYS[1])
if gva == ARGV[1] then
  local ttlva = redis.call('PTTL', KEYS[1])
  redis.call('PEXPIRE', KEYS[1], ARGV[2] + ttlva)
  return 1
end
return 0", _name, _value, milliseconds)?.ToString() == "1";
            if (ret == false) _autoDelayTimer?.Dispose(); //未知情况，关闭定时器
            return ret;
        }

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        /// <returns>成功/失败</returns>
        public bool Unlock()
        {
            _autoDelayTimer?.Dispose();
            return _client.Eval(@"local gva = redis.call('GET', KEYS[1])
if gva == ARGV[1] then
  redis.call('DEL', KEYS[1])
  return 1
end
return 0", _name, _value)?.ToString() == "1";
        }

        public void Dispose() => this.Unlock();
    }
}