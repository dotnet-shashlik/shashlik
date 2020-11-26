using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    [AutoOptions("Shashlik.DataProtection.PostgreSql")]
    public class PostgreSqlDataProtectionOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 允许自动创建数据库
        /// </summary>
        public bool EnableAutoCreateDataBase = true;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// scheme
        /// </summary>
        public string Scheme { get; set; } = "public";

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