using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.Identity
{
    [AutoOptions("Shashlik:Identity")]
    public class ShashlikIdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }
    }
}