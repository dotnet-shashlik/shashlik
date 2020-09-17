using Shashlik.Kernel.Autowired.Attributes;

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

        /// <summary>
        /// 启用redis分布式锁
        /// </summary>
        public bool EnableRedisLock { get; set; }

        /*
         * 典型的哨兵模式配置
         * ConnectionString: mymaster,password=123,prefix=my_
         * Sentinels: new [] { "192.169.1.10:26379", "192.169.1.11:26379", "192.169.1.12:26379" })
         * **/

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 哨兵配置
        /// </summary>
        public string[] Sentinels { get; set; }

        /// <summary>
        /// 只读
        /// </summary>
        public bool Readonly { get; set; }
    }
}