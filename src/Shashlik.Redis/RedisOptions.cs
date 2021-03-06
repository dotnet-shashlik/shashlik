﻿using System;
using CSRedis;
using Shashlik.Kernel.Attributes;

// ReSharper disable All

namespace Shashlik.Redis
{
    [AutoOptions("Shashlik.Redis")]
    public class RedisOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /*
         * 典型的哨兵模式配置
         * ConnectionString: mymaster,password=123,prefix=my_
         * Sentinels: new [] { "192.169.1.10:26379", "192.169.1.11:26379", "192.169.1.12:26379" })
         * **/

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 哨兵配置
        /// </summary>
        public string[] Sentinels { get; set; } = new string[0];

        /// <summary>
        /// 只读
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// 自定义Client创建，高优先级
        /// </summary>
        public Func<CSRedisClient>? CSRedisClientFactory { get; set; }
    }
}