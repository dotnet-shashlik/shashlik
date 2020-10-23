using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel;

namespace Shashlik.Identity
{
    /// <summary>
    /// identity扩展配置
    /// </summary>
    public interface IIdentityExtensionAutowire : IAutowire
    {
        void Configure(IdentityBuilder identityBuilder);
    }
}