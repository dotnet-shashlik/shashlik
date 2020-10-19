using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.DataProtection
{
    [AutoOptions("Shashlik.DataProtector.PostgreSql")]
    public class PostgreSqlDataProtectorOptions
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
        public string TableName { get; set; } = "ShashlikDataProtectionKeys";

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; } = "DefaultApplicationName";
    }
}