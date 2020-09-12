using NPOI.Util.Collections;
using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.EfCore
{
    [AutoOptions("Shashlik.EfCore")]
    public class EfCoreOptions
    {
        /// <summary>
        /// 是否启用efcore自动配置,默认true
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 是否全部ShashlikDbContext自动迁移,将自动扫描程序所有的ShashlikDbContext并执行迁移
        /// </summary>
        public bool AutoMigrationAll { get; set; } = false;

        /// <summary>
        /// 允许启动自动迁移的类型,<see cref="AutoMigrationAll"/>=false有效
        /// </summary>
        public HashSet<string> AutoMigrationTypes { get; set; } = new HashSet<string>();
    }
}