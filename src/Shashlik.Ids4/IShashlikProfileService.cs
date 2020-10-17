using IdentityServer4.Services;

namespace Shashlik.Ids4
{
    /// <summary>
    /// 自定义profile, 将自定义加载,profile不能重复配置
    /// </summary>
    public interface IShashlikProfileService : IProfileService
    {
    }
}