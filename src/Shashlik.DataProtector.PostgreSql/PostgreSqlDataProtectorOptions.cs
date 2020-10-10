using Shashlik.Kernel.Autowired.Attributes;

namespace Shashlik.DataProtector.PostgreSql
{
    [AutoOptions("Shashlik.DataProtector.PostgreSql")]
    public class PostgreSqlDataProtectorOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 缓存key
        /// </summary>
        public string Key { get; set; } = "Shashlik-DataProtection-Keys";

        /// <summary>
        /// 应用程序名称
        /// </summary>
        public string ApplicationName { get; set; } = "DefaultApplicationName";
    }
}