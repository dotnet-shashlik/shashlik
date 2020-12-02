using Shashlik.Identity.DataProtection;
using Shashlik.Kernel.Attributes;

namespace Shashlik.Identity.Options
{
    [AutoOptions("Shashlik.Identity")]
    public class ShashlikIdentityOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 数据库版本，目前mysql需要
        /// </summary>
        public string? DbVersion { get; set; }

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 迁移的程序集名称,如果更改 UserProperty或者identity默认项的配置,需要自行生成迁移,可以传入此参数使用传入的程序集进行自动迁移
        /// </summary>
        public string? MigrationAssembly { get; set; }

        /// <summary>
        /// 验证码长度
        /// </summary>
        public int CaptchaLength { get; set; } = 6;

        /// <summary>
        /// 数据保护token配置项
        /// </summary>
        public DataProtectionTokenProviderOptions DataProtectionTokenProviderOptions { get; set; } =
            new DataProtectionTokenProviderOptions();
    }
}