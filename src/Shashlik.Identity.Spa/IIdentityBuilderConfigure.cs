using Microsoft.AspNetCore.Identity;

namespace Shashlik.Identity.Spa
{
    /// <summary>
    /// identity扩展配置
    /// </summary>
    public interface IIdentityBuilderConfigure
    {
        /// <summary>
        /// identity扩展配置
        /// </summary>
        /// <param name="builder"></param>
        void Configure(IdentityBuilder builder);
    }
}