using Shashlik.Kernel.Attributes;

namespace Shashlik.Ids4.MySqlStore
{
    [AutoOptions("Shashlik.Ids4.MySql")]
    public class Ids4MySqlStoreOptions
    {
        /// <summary>
        /// 自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 数据库版本,默认5.7
        /// </summary>
        public string? DbVersion { get; set; } = "5.7";

        /// <summary>
        /// 是否使用配置efcore存储,如果系统的客户端数据比较固定,推荐使用内存配置;如果系统的客户端数据需要动态删减,最好使用efcore数据库配置
        /// </summary>
        public bool EnableConfigurationStore { get; set; }

        /// <summary>
        /// 操作存储,引用token,设备流需要,如果没这方面的应用可以不使用
        /// </summary>
        public bool EnableOperationalStore { get; set; }
    }
}