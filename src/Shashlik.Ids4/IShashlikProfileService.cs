using IdentityServer4.Services;
using Shashlik.Kernel.Dependency;

namespace Shashlik.Ids4
{
    /// <summary>
    /// 自定义profile, 将自动加载,profile不能重复配置
    /// </summary>
    [Transient]
    public interface IShashlikProfileService : IProfileService
    {
    }
}