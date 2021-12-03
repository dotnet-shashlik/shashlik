using Shashlik.Kernel.Attributes;

namespace Shashlik.Sms.Limit.Redis
{
    [AutoOptions("Shashlik.Sms")]
    public class RedisSmsLimitOptions
    {
        public bool EnableRedisLimit { get; set; } = true;
    }
}