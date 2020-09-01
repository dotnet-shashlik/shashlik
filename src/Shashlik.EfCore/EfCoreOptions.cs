using Shashlik.Kernel.Autowire.Attributes;

namespace Shashlik.EfCore
{
    [AutoOptions("Shashlik:EfCore")]
    public class EfCoreOptions
    {
        /// <summary>
        /// 是否启用efcore自动配置,默认true
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}