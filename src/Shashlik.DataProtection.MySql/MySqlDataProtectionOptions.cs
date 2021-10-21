#nullable disable
using Shashlik.Kernel.Attributes;

// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace
namespace Shashlik.DataProtection
{
    [AutoOptions("Shashlik.DataProtection.MySql")]
    public class MySqlDataProtectionOptions
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
        /// 数据库连接字符串, 默认使用ConnectionStrings.Default
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