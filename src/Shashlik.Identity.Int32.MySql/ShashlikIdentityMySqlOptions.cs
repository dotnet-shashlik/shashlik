﻿// ReSharper disable CheckNamespace

using Shashlik.Kernel.Attributes;

namespace Shashlik.Identity
{
    [AutoOptions("Shashlik.Identity.MySql")]
    public class ShashlikIdentityMySqlOptions
    {
        /// <summary>
        /// 数据库连接字符串，默认读取 ConnectionString:Default节点
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// 数据库版本，目前mysql需要, 默认5.7
        /// </summary>
        public string? DbVersion { get; set; } = "5.7";

        /// <summary>
        /// 是否自动迁移
        /// </summary>
        public bool AutoMigration { get; set; }

        /// <summary>
        /// 迁移的程序集名称,如果更改 UserProperty或者identity默认项的配置,需要自行生成迁移,可以传入此参数使用传入的程序集进行自动迁移
        /// </summary>
        public string? MigrationAssembly { get; set; }
    }
}