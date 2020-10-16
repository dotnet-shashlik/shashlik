using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.DataProtector.MySql
{
    [AutoOptions("Shashlik.DataProtector.MySql")]
    public class MySqlDataProtectorOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; } = "shashlik.data.protection.keys";

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; } = "DefaultApplicationName";
    }
}