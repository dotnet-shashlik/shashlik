using System;
using System.Collections.Generic;
using System.Text;
using Shashlik.Kernel.Attributes;

namespace Shashlik.EventBus
{
    [AutoOptions("Shashlik.EventBus")]
    public class EventBusOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// CAP默认组名
        /// </summary>
        public string? DefaultGroup { get; set; }

        /// <summary>
        /// CAP当前版本名称,一般用于区分不用的环境
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// 成功的消息过期时间:秒,空则使用CAP默认配置
        /// </summary>
        public int? SucceedMessageExpiredAfter { get; set; }

        /// <summary>
        /// 失败重试间隔:秒,空则使用CAP默认配置
        /// </summary>
        public int? FailedRetryInterval { get; set; }

        /// <summary>
        /// 失败重试次数,空则使用CAP默认配置
        /// </summary>
        public int? FailedRetryCount { get; set; }

        /// <summary>
        /// 消费者线程数量,空则使用CAP默认配置
        /// </summary>
        public int? ConsumerThreadCount { get; set; }
    }
}