using Shashlik.Kernel.Attributes;

// ReSharper disable CheckNamespace

namespace Shashlik.DataProtection
{
    [AutoOptions("Shashlik.DataProtector.Redis")]
    public class RedisDataProtectorOptions
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