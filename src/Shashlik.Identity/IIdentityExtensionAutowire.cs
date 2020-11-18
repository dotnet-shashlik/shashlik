using Microsoft.AspNetCore.Identity;
using Shashlik.Kernel;
using Shashlik.Kernel.Dependency;

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