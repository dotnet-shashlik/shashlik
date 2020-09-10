using System.Collections.Generic;
using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Sms.Options
{
    /// <summary>
    /// 短信配置
    /// </summary>
    [AutoOptions("Shashlik.Sms")]
    public class SmsOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 使用空短信,不真实发送
        /// </summary>
        public bool UseEmptySms { get; set; }

        /// <summary>
        /// 启用分布式缓存短信发送限制,需自行配置分布式缓存
        /// </summary>
        public bool EnableDistributedCacheLimit { get; set; }

        /// <summary>
        /// 批量发送最大手机数量
        /// </summary>
        public int Max { get; set; } = 1000;

        /// <summary>
        /// 短信发送限制
        /// </summary>
        public List<SmsLimitConfig> Limits { get; set; }

        /// <summary>
        /// 短信配置
        /// </summary>
        public List<SmsDomainConfig> DomainConfigs { get; set; }
    }
}