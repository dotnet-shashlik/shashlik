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

        /// <summary>
        /// 全局token生命周期,单位分,默认2小时(官方默认为1天).如需对不同的provider进行过期设置见:https://docs.microsoft.com/zh-cn/aspnet/core/security/authentication/accconfirm?view=aspnetcore-3.1&tabs=visual-studio
        /// </summary>
        public int TokenLifespan { get; set; } = 120;
    }
}