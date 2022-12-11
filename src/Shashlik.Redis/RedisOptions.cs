using Shashlik.Kernel.Attributes;

namespace Shashlik.Redis;

[AutoOptions("Shashlik.Redis")]
public class RedisOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enable { get; set; } = true;

    /// <summary>
    /// 0:单机/主从, 1:哨兵, 2:集群
    /// </summary>
    public RedisMode Mode { get; set; } = RedisMode.Default;

    /// <summary>
    /// 哨兵模式是否读写分离
    /// </summary>
    public bool RwSplitting { get; set; }

    /// <summary>
    /// 主从/单机连接字符串
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// 哨兵
    /// </summary>
    public string[]? Sentinels { get; set; }

    /// <summary>
    /// 从
    /// </summary>
    public string[]? SlaveConnectionStrings { get; set; }

    /// <summary>
    /// 集群
    /// </summary>
    public string[]? ClusterConnectionStrings { get; set; }

    public enum RedisMode
    {
        /// <summary>
        /// 默认: 单机/主从
        /// </summary>
        Default = 0,

        /// <summary>
        /// 哨兵模式
        /// </summary>
        Sentinel = 1,

        /// <summary>
        /// 集群模式
        /// </summary>
        Cluster = 2
    }
}